using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Dalamud.Bindings.ImGui;
using Dalamud.Utility;
using SortaKinda.Configuration;

namespace SortaKinda.Classes;

/// <summary>
/// Class for helping with loading/saving and copying/exporting rulesets to clipboard.
/// </summary>
public static class PresetManager {
	private static readonly JsonSerializerOptions SerializerOptions = new() {
		IncludeFields = true,
	};

	/// <summary>
	/// Loads code from clipboard and imports rules, will check the ruleset id and won't allow duplicate ruleset id's.
	/// </summary>
	public static void LoadFromClipboard() {
		try {
			var decodedString = Convert.FromBase64String(ImGui.GetClipboardText());
			var uncompressed = Util.DecompressString(decodedString);

			if (uncompressed.IsNullOrEmpty()) {
				Services.ChatGui.PrintError("[Import] Tried to import sorting rules, but got nothing, try copying the code again.", "SortaKinda");
				return;
			}

			// Older (pre-3.0.0) backup codes use a different layout. Detect and convert them
			// transparently so the user never has to pick a format or know the difference.
			if (LegacyPresetMigrator.IsLegacyFormat(uncompressed)) {
				HandleLegacyImport(uncompressed);
				return;
			}

			if (JsonSerializer.Deserialize<PresetContainer>(uncompressed, SerializerOptions) is { } clipboardData) {
				clipboardData.RuleSets.RemoveAll(ruleSet => ruleSet.RuleSetId == SlotSet.IgnoreSlotsId);

				if (clipboardData.RuleSets.Count is 0) {
					Services.ChatGui.PrintError("[Import] Tried to import sorting rules, but got nothing, try copying the code again.", "SortaKinda");
					return;
				}

				// Remove any invalid filter or orderings.
				foreach (var ruleSet in clipboardData.RuleSets) {
					ruleSet.FilterRules
					       .RemoveAll(filter => !filter.IsValid);

					ruleSet.OrderingRules
					       .RemoveAll(filter => !filter.IsValid);
				}

				var addedCount = 0;
				foreach (var rule in clipboardData.RuleSets) {
					if (System.SystemConfiguration.RuleSets.All(existingRule => existingRule.RuleSetId != rule.RuleSetId)) {
						System.SystemConfiguration.RuleSets.Add(rule);
						addedCount++;
					}
				}

				Services.ChatGui.Print($"[Import] Received {clipboardData.RuleSets.Count} sorting rules from clipboard. ", "SortaKinda");
				Services.ChatGui.Print($"[Import] Added {addedCount} new sorting rules.", "SortaKinda");
				System.SystemConfiguration.Save(false);
			}
		}
		catch (Exception e) {
			Services.PluginLog.Error(e, "Error Parsing Preset");
			Services.ChatGui.PrintError("[Import] Something went wrong trying to import rulesets, check you copied the code correctly.", "SortaKinda");
		}

	}

	/// <summary>
	/// Converts a legacy (2.x) backup code into the current model and imports it,
	/// restoring both the sorting rules and (when logged in) the slot layout.
	/// </summary>
	private static void HandleLegacyImport(string json) {
		if (!LegacyPresetMigrator.TryParse(json, out var legacyExport)) {
			Services.ChatGui.PrintError("[Import] Tried to import an older backup code, but couldn't read any rules from it.", "SortaKinda");
			return;
		}

		var migratedRuleSets = LegacyPresetMigrator.BuildRuleSets(legacyExport);

		foreach (var ruleSet in migratedRuleSets) {
			ruleSet.FilterRules
			       .RemoveAll(filter => !filter.IsValid);

			ruleSet.OrderingRules
			       .RemoveAll(ordering => !ordering.IsValid);
		}

		// A rule with no filters used to mean "match every item" in the old version. That behavior
		// is intentionally unsupported now, so any such rule is dropped and reported to the user.
		var droppedRuleSets = migratedRuleSets
			.Where(ruleSet => ruleSet.FilterRules.Count is 0)
			.ToList();

		var importableRuleSets = migratedRuleSets
			.Where(ruleSet => ruleSet.FilterRules.Count > 0)
			.ToList();

		var addedCount = 0;
		var updatedCount = 0;
		foreach (var ruleSet in importableRuleSets) {
			var existingIndex = System.SystemConfiguration.RuleSets.FindIndex(existingRule => existingRule.RuleSetId == ruleSet.RuleSetId);
			if (existingIndex >= 0) {
				System.SystemConfiguration.RuleSets[existingIndex] = ruleSet;
				updatedCount++;
			}
			else {
				System.SystemConfiguration.RuleSets.Add(ruleSet);
				addedCount++;
			}
		}

		var inventoriesRestored = ApplyLegacySlotLayout(legacyExport);

		System.SystemConfiguration.Save(false);
		CopyToClipboard(importableRuleSets);

		Services.ChatGui.Print("[Import] Detected an older SortaKinda backup code and converted it automatically.", "SortaKinda");
		Services.ChatGui.Print($"[Import] Added {addedCount} new sorting rules and updated {updatedCount} existing rules (backup contained {migratedRuleSets.Count}).", "SortaKinda");

		foreach (var dropped in droppedRuleSets) {
			Services.ChatGui.PrintError($"[Import] Rule Set '{dropped.Name}' was dropped because it had no filters. The old 'match all items' behavior is no longer supported.", "SortaKinda");
		}

		Services.ChatGui.Print("[Import] Replaced the clipboard contents with a new-format backup code.", "SortaKinda");

		if (inventoriesRestored > 0) {
			Services.ChatGui.Print($"[Import] Restored slot assignments for {inventoriesRestored} inventories.", "SortaKinda");
		}
	}

	/// <summary>
	/// Rebuilds the current character's slot layout from a legacy export. Requires a logged in character.
	/// </summary>
	/// <returns>The number of inventories that had their slot assignments restored.</returns>
	private static int ApplyLegacySlotLayout(LegacyExport legacyExport) {
		if (System.CharacterConfiguration is not { } characterConfiguration) {
			return 0;
		}

		var knownRuleSetIds = System.SystemConfiguration.RuleSets
			.Select(ruleSet => ruleSet.RuleSetId)
			.ToHashSet();

		var migratedSlotSets = LegacyPresetMigrator.BuildSlotSets(legacyExport, knownRuleSetIds);
		if (migratedSlotSets.Count is 0) {
			return 0;
		}

		foreach (var (inventoryType, slotSets) in migratedSlotSets) {
			if (!characterConfiguration.Inventories.TryGetValue(inventoryType, out var inventoryConfig)) {
				inventoryConfig = new InventoryConfig();
				characterConfiguration.Inventories[inventoryType] = inventoryConfig;
			}

			inventoryConfig.SlotSets = slotSets;
		}

		characterConfiguration.Save();
		return migratedSlotSets.Count;
	}

	/// <summary>
	/// Saves all the current rulesets to encoded text data to clipboard
	/// </summary>
	public static void SaveToClipboard() {
		var data = CreatePresetContainer(System.SystemConfiguration.RuleSets
			.Where(ruleSet => ruleSet.RuleSetId != SlotSet.IgnoreSlotsId));

		CopyToClipboard(data.RuleSets);
		Services.ChatGui.Print($"[Export] Exported {data.RuleSets.Count} rules to clipboard.", "SortaKinda");
	}

	private static void CopyToClipboard(IEnumerable<RuleSet> ruleSets) {
		var data = CreatePresetContainer(ruleSets);
		var jsonString = JsonSerializer.Serialize(data, SerializerOptions);

		var compressed = Util.CompressString(jsonString);
		ImGui.SetClipboardText(Convert.ToBase64String(compressed));
	}

	private static PresetContainer CreatePresetContainer(IEnumerable<RuleSet> ruleSets) => new() {
		RuleSets = ruleSets
			.Where(ruleSet => ruleSet.RuleSetId != SlotSet.IgnoreSlotsId)
			.ToList(),
	};
}