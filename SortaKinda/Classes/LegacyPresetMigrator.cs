using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Configuration;
using SortaKinda.FilterRules;
using SortaKinda.OrderRules;

namespace SortaKinda.Classes;

/// <summary>
/// Converts a pre-3.0.0 ("2.x") SortaKinda export into the current configuration model.
/// Used so old backup codes can still be imported seamlessly without any user interaction.
/// </summary>
internal static class LegacyPresetMigrator {
	private const string DefaultRuleId = "Default";

	// Namespace for stable GUIDs when a legacy rule id is not already a GUID string.
	// Used as a base id that is mutated when importing individual rules.
	private static readonly Guid LegacyIdNamespace = new("8f4e2c1a-9b3d-4f6e-a5c8-2d7e9f0b1c3a");

	private static readonly JsonSerializerOptions LegacyOptions = new() {
		IncludeFields = true,
		PropertyNameCaseInsensitive = true,
	};

	/// <summary>
	/// Returns true when the given JSON looks like a legacy (2.x) export rather than a current preset.
	/// </summary>
	public static bool IsLegacyFormat(string json) {
		try {
			using var document = JsonDocument.Parse(json);
			var root = document.RootElement;

			if (root.ValueKind is not JsonValueKind.Object) {
				return false;
			}

			// Current presets are wrapped in a PresetContainer that has "RuleSets".
			if (root.TryGetProperty("RuleSets", out _)) {
				return false;
			}

			// Legacy exports always carry a flat "Rules" array.
			return root.TryGetProperty("Rules", out var rules) && rules.ValueKind is JsonValueKind.Array;
		}
		catch (JsonException) {
			return false;
		}
	}

	public static bool TryParse(string json, out LegacyExport export) {
		export = new LegacyExport();

		try {
			if (JsonSerializer.Deserialize<LegacyExport>(json, LegacyOptions) is { } parsed) {
				export = parsed;
				return export.Rules.Count > 0;
			}
		}
		catch (JsonException e) {
			IPluginLog.Get().Error(e, "Failed to parse legacy SortaKinda export.");
		}

		return false;
	}

	/// <summary>
	/// Converts every legacy rule into a current <see cref="RuleSet"/>.
	/// </summary>
	public static List<RuleSet> BuildRuleSets(LegacyExport export)
		=> export.Rules
			.Where(IsImportableLegacyRule)
			.Select(BuildRuleSet)
			.ToList();

	private static bool IsImportableLegacyRule(LegacyRule rule)
		=> !string.IsNullOrWhiteSpace(rule.Id) && !string.Equals(rule.Id, DefaultRuleId, StringComparison.Ordinal);

	private static RuleSet BuildRuleSet(LegacyRule rule) => new() {
		Name = rule.Name,
		RuleSetId = ParseLegacyId(rule.Id),
		Color = rule.Color,
		RequireAll = !rule.InclusiveAnd,
		ReverseFill = rule.FillMode == 1,
		FilterRules = BuildFilterRules(rule),
		OrderingRules = BuildOrderingRules(rule),
	};

	private static List<FilteringRuleBase> BuildFilterRules(LegacyRule rule) {
		var filters = new List<FilteringRuleBase>();

		if (rule.AllowedItemTypes.Count > 0) {
			filters.Add(new ItemUiCategoryFilter {
				ItemUiCategories = rule.AllowedItemTypes.ToList(),
			});
		}

		var names = ExtractRegexStrings(rule.AllowedNameRegexes);
		if (names.Count > 0) {
			filters.Add(new NameFilter {
				Names = names,
			});
		}

		if (rule.AllowedItemRarities.Count > 0) {
			filters.Add(new RarityFilter {
				Rarities = rule.AllowedItemRarities.ToList(),
			});
		}

		if (rule.LevelFilter is { Enable: true } equipLevel) {
			filters.Add(new EquipLevelFilter {
				MinLevel = ToUInt(equipLevel.MinValue),
				MaxLevel = ToUInt(equipLevel.MaxValue),
			});
		}

		if (rule.ItemLevelFilter is { Enable: true } itemLevel) {
			filters.Add(new ItemLevelFilter {
				MinLevel = ToUInt(itemLevel.MinValue),
				MaxLevel = ToUInt(itemLevel.MaxValue),
			});
		}

		if (rule.VendorPriceFilter is { Enable: true } vendorPrice) {
			filters.Add(new VendorPriceFilter {
				MinPrice = ToUInt(vendorPrice.MinValue),
				MaxPrice = ToUInt(vendorPrice.MaxValue),
			});
		}

		AddToggleFilter(filters, rule.UntradableFilter, static () => new UntradableFilter());
		AddToggleFilter(filters, rule.UniqueFilter, static () => new UniqueFilter());
		AddToggleFilter(filters, rule.CollectableFilter, static () => new CollectableFilter());
		AddToggleFilter(filters, rule.DyeableFilter, static () => new DyeableFilter());
		AddToggleFilter(filters, rule.RepairableFilter, static () => new RepairableFilter());

		return filters;
	}

	private static void AddToggleFilter(List<FilteringRuleBase> filters, LegacyToggleFilter? toggle, Func<FilteringRuleBase> factory) {
		// State: 0 = Ignored (skip), 1 = Allow, 2 = Disallow
		if (toggle is null || toggle.State == 0) {
			return;
		}

		var filter = factory();
		filter.IsAllowed = toggle.State == 1;
		filters.Add(filter);
	}

	private static List<OrderingRuleBase> BuildOrderingRules(LegacyRule rule) {
		var orderings = new List<OrderingRuleBase>();

		AddOrdering(orderings, rule.SortMode, rule.Direction);

		if (rule.AdditionalSortRules.Count > 0) {
			foreach (var additional in rule.AdditionalSortRules) {
				AddOrdering(orderings, additional.Mode, additional.Direction);
			}
		}
		else {
			// Older configs stored only modes and reused the primary direction.
			foreach (var mode in rule.AdditionalSortModes) {
				AddOrdering(orderings, mode, rule.Direction);
			}
		}

		return orderings;
	}

	private static void AddOrdering(List<OrderingRuleBase> orderings, int sortMode, int direction) {
		if (CreateOrdering(sortMode, direction) is not { } ordering) {
			return;
		}

		// The current UI only allows one ordering of each type; mirror that here.
		if (orderings.Any(existing => existing.GetType() == ordering.GetType())) {
			return;
		}

		orderings.Add(ordering);
	}

	private static OrderingRuleBase? CreateOrdering(int sortMode, int direction) {
		// In the legacy model Ascending (0) always meant "low value first" and
		// Descending (1) "high value first". Each current ordering has its own default
		// direction, so we flip IsReversed only when the defaults disagree.
		var wantDescending = direction == 1;

		var (ordering, defaultIsDescending) = sortMode switch {
			0 => ((OrderingRuleBase)new AlphabeticalOrdering(), false),
			1 => (new ItemLevelOrdering(), true),
			2 => (new RarityOrdering(), false),
			3 => (new SellPriceOrdering(), false),
			4 => (new ItemIdOrdering(), true),
			5 => (new ItemUiCategoryOrdering(), false),
			6 => (new EquipLevelOrdering(), true),
			_ => (null, false),
		};

		if (ordering is null) {
			return null;
		}

		ordering.IsReversed = wantDescending != defaultIsDescending;
		return ordering;
	}

	/// <summary>
	/// Rebuilds the per-inventory slot layout from a legacy export, keyed by inventory type.
	/// Only rule ids present in <paramref name="knownRuleSetIds"/> are kept.
	/// </summary>
	public static Dictionary<InventoryType, List<SlotSet>> BuildSlotSets(LegacyExport export, HashSet<Guid> knownRuleSetIds) {
		var result = new Dictionary<InventoryType, List<SlotSet>>();
		var rulePriorities = export.Rules
			.Where(IsImportableLegacyRule)
			.Select(rule => (RuleSetId: ParseLegacyId(rule.Id), rule.Index))
			.ToDictionary(entry => entry.RuleSetId, entry => entry.Index);

		AddInventoryRoot(result, export.MainInventory, knownRuleSetIds, rulePriorities);
		AddInventoryRoot(result, export.Armory, knownRuleSetIds, rulePriorities);

		return result;
	}

	private static void AddInventoryRoot(
		Dictionary<InventoryType, List<SlotSet>> result,
		LegacyInventoryRoot? root,
		HashSet<Guid> knownRuleSetIds,
		Dictionary<Guid, int> rulePriorities
		)
	{
		if (root is null) {
			return;
		}

		foreach (var config in root.InventoryConfigs) {
			var inventoryType = (InventoryType)config.Type;
			if (!System.AllowedInventories.Contains(inventoryType)) {
				continue;
			}

			var slotSets = new List<SlotSet>();

			var groupedSlots = config.SlotConfigs
				.Select((slot, index) => (slot.RuleId, index))
				.Where(entry => !string.IsNullOrEmpty(entry.RuleId) && entry.RuleId != DefaultRuleId)
				.GroupBy(entry => entry.RuleId);

			foreach (var group in groupedSlots) {
				var ruleSetId = ParseLegacyId(group.Key);

				if (!knownRuleSetIds.Contains(ruleSetId)) {
					continue;
				}

				slotSets.Add(new SlotSet {
					InventoryType = inventoryType,
					RuleSetId = ruleSetId,
					SlotIndexes = group.Select(entry => entry.index).ToList(),
					Priority = rulePriorities.GetValueOrDefault(ruleSetId),
				});
			}

			if (slotSets.Count > 0) {
				result[inventoryType] = slotSets;
			}
		}
	}

	private static List<string> ExtractRegexStrings(List<JsonElement> elements) {
		var results = new List<string>();

		foreach (var element in elements) {
			switch (element.ValueKind) {
				case JsonValueKind.String when element.GetString() is { Length: > 0 } stringValue:
					TryAddValidRegex(results, stringValue);
					break;

				case JsonValueKind.Object: {
					foreach (var propertyName in (string[]) ["RegexString", "Regex", "Pattern", "Value"]) {
						if (element.TryGetProperty(propertyName, out var property)
						    && property.ValueKind is JsonValueKind.String
						    && property.GetString() is { Length: > 0 } objectValue) {
							TryAddValidRegex(results, objectValue);
							break;
						}
					}

					break;
				}
			}
		}

		return results;
	}

	private static void TryAddValidRegex(List<string> results, string pattern) {
		if (!IsValidRegexPattern(pattern)) {
			IPluginLog.Get().Warning($"Skipping invalid legacy name regex during import: {pattern}");
			return;
		}

		results.Add(pattern);
	}

	private static bool IsValidRegexPattern(string pattern) {
		try {
			_ = new Regex(pattern);
			return true;
		}
		catch (ArgumentException) {
			return false;
		}
	}

	private static Guid ParseLegacyId(string id) {
		if (Guid.TryParse(id, out var guid)) {
			return guid;
		}

		var hash = MD5.HashData(Encoding.UTF8.GetBytes($"{LegacyIdNamespace:N}:{id}"));
		hash[6] = (byte)(hash[6] & 0x0F | 0x30);
		hash[8] = (byte)(hash[8] & 0x3F | 0x80);
		return new Guid(hash);
	}

	private static uint ToUInt(int value)
		=> value < 0 ? 0u : (uint)value;
}
