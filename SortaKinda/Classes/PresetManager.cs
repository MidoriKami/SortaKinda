using System;
using System.Linq;
using System.Text.Json;
using Dalamud.Bindings.ImGui;
using Dalamud.Utility;

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

			if (JsonSerializer.Deserialize<PresetContainer>(uncompressed, SerializerOptions) is { } clipboardData) {
				if (clipboardData.RuleSets.Count is 0) {
					Services.ChatGui.PrintError("[Import] Tried to import sorting rules, but got nothing, try copying the code again.", "SortaKinda");
					return;
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
	/// Saves all the current rulesets to encoded text data to clipboard
	/// </summary>
	public static void SaveToClipboard() {
		var data = new PresetContainer {
			RuleSets = System.SystemConfiguration.RuleSets,
		};

		var jsonString = JsonSerializer.Serialize(data, SerializerOptions);

		var compressed = Util.CompressString(jsonString);
		ImGui.SetClipboardText(Convert.ToBase64String(compressed));

		Services.ChatGui.Print($"[Export] Exported {data.RuleSets.Count} rules to clipboard.", "SortaKinda");
	}
}