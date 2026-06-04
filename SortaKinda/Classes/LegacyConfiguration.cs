using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;

namespace SortaKinda.Classes;

/// <summary>
/// Data-transfer objects that mirror the pre-3.0.0 ("2.x") SortaKinda export/config layout.
/// These are only used to read legacy data so it can be migrated into the current model.
/// </summary>
internal class LegacyExport {
	public List<LegacyRule> Rules { get; set; } = [];
	public LegacyInventoryRoot? MainInventory { get; set; }
	public LegacyInventoryRoot? Armory { get; set; }
}

internal class LegacyRule {
	public Vector4 Color { get; set; } = Vector4.One;
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = "New Rule";
	public int Index { get; set; }

	public List<JsonElement> AllowedNameRegexes { get; set; } = [];
	public List<uint> AllowedItemTypes { get; set; } = [];
	public List<uint> AllowedItemRarities { get; set; } = [];

	public LegacyRangeFilter? LevelFilter { get; set; }
	public LegacyRangeFilter? ItemLevelFilter { get; set; }
	public LegacyRangeFilter? VendorPriceFilter { get; set; }

	public LegacyToggleFilter? UntradableFilter { get; set; }
	public LegacyToggleFilter? UniqueFilter { get; set; }
	public LegacyToggleFilter? CollectableFilter { get; set; }
	public LegacyToggleFilter? DyeableFilter { get; set; }
	public LegacyToggleFilter? RepairableFilter { get; set; }

	// 0 = Ascending, 1 = Descending
	public int Direction { get; set; }

	// 0 = Top, 1 = Bottom
	public int FillMode { get; set; }

	// SortOrderMode: 0 Alphabetical, 1 ItemLevel, 2 Rarity, 3 SellPrice, 4 ItemId, 5 ItemType, 6 Level(equip)
	public int SortMode { get; set; }

	public List<int> AdditionalSortModes { get; set; } = [];
	public List<LegacyAdditionalSortRule> AdditionalSortRules { get; set; } = [];

	public bool InclusiveAnd { get; set; }
}

internal class LegacyRangeFilter {
	public bool Enable { get; set; }
	public int MinValue { get; set; }
	public int MaxValue { get; set; }
}

internal class LegacyToggleFilter {
	// ToggleFilterState: 0 Ignored, 1 Allow, 2 Disallow
	public int State { get; set; }
	public int Filter { get; set; }
}

internal class LegacyAdditionalSortRule {
	public int Mode { get; set; }
	public int Direction { get; set; }
}

internal class LegacyInventoryRoot {
	public List<LegacyInventoryConfig> InventoryConfigs { get; set; } = [];
}

internal class LegacyInventoryConfig {
	public List<LegacySlotConfig> SlotConfigs { get; set; } = [];

	// For main inventory this is 0-3 (Inventory1-4); for armory it is the raw InventoryType value.
	public int Type { get; set; }
}

internal class LegacySlotConfig {
	public string RuleId { get; set; } = string.Empty;
}
