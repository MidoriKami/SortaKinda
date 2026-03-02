using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.Classes;
using Lumina.Excel.Sheets;
using SortaKinda.Controllers;
using SortaKinda.ViewComponents;

namespace SortaKinda.Classes;

public unsafe class SortingRule : IComparer<InventorySlot>{
    private readonly SortingRuleTooltipView view;
    private readonly List<SortingFilter> filterRules;

    public SortingRule() {
        view = new SortingRuleTooltipView(this);
        filterRules = new List<SortingFilter> {
            new() {
                Active = () => AllowedNameRegexes.Count != 0,
                IsSlotAllowed = slot => {
                    foreach (var allowedRegex in AllowedNameRegexes) {
                        if (slot is { ExdItem: { Name: var itemName, RowId: not 0 } }) {
                            if (allowedRegex.Match(itemName.ExtractText())) return true;
                        }
                    }

                    return false;
                },
            },
            new() {
                Active = () => AllowedItemTypes.Count != 0,
                IsSlotAllowed = slot => AllowedItemTypes.Any(allowed => slot.ExdItem.ItemUICategory.RowId == allowed),
            },
            new() {
                Active = () => AllowedItemRarities.Count != 0,
                IsSlotAllowed = slot => AllowedItemRarities.Any(allowed => slot.ExdItem.Rarity == (byte) allowed),
            },
            new() {
                Active = () => LevelFilter.Enable,
                IsSlotAllowed = slot => LevelFilter.IsItemSlotAllowed(slot.ExdItem.LevelEquip),
            },
            new() {
                Active = () => ItemLevelFilter.Enable,
                IsSlotAllowed = slot => ItemLevelFilter.IsItemSlotAllowed(slot.ExdItem.LevelItem.RowId),
            },
            new() {
                Active = () => VendorPriceFilter.Enable,
                IsSlotAllowed = slot => VendorPriceFilter.IsItemSlotAllowed(slot.ExdItem.PriceLow),
            },
            new() {
                Active = () => UntradableFilter.State is not ToggleFilterState.Ignored,
                IsSlotAllowed = slot => UntradableFilter.IsItemSlotAllowed(slot),
            },
            new() {
                Active = () => UniqueFilter.State is not ToggleFilterState.Ignored,
                IsSlotAllowed = slot => UniqueFilter.IsItemSlotAllowed(slot),
            },
            new() {
                Active = () => CollectableFilter.State is not ToggleFilterState.Ignored,
                IsSlotAllowed = slot => CollectableFilter.IsItemSlotAllowed(slot),
            },
            new() {
                Active = () => DyeableFilter.State is not ToggleFilterState.Ignored,
                IsSlotAllowed = slot => DyeableFilter.IsItemSlotAllowed(slot),
            },
            new() {
                Active = () => RepairableFilter.State is not ToggleFilterState.Ignored,
                IsSlotAllowed = slot => RepairableFilter.IsItemSlotAllowed(slot),
            }
        };
    }

    public Vector4 Color { get; set; }
    public string Id { get; set; } = SortController.DefaultId;
    public string Name { get; set; } = "New Rule";
    public int Index { get; set; }
    public HashSet<UserRegex> AllowedNameRegexes { get; set; } = [];
    public HashSet<uint> AllowedItemTypes { get; set; } = [];
    public HashSet<ItemRarity> AllowedItemRarities { get; set; } = [];
    public RangeFilter LevelFilter { get; set; } = new("Level Filter", 0, 200);
    public RangeFilter ItemLevelFilter { get; set; } = new("Item Level Filter", 0, 1000);
    public RangeFilter VendorPriceFilter { get; set; } = new("Vendor Price Filter", 0, 1_000_000);
    public ToggleFilter UntradableFilter { get; set; } = new(PropertyFilter.Untradable);
    public ToggleFilter UniqueFilter { get; set; } = new(PropertyFilter.Unique);
    public ToggleFilter CollectableFilter { get; set; } = new(PropertyFilter.Collectable);
    public ToggleFilter DyeableFilter { get; set; } = new(PropertyFilter.Dyeable);
    public ToggleFilter RepairableFilter { get; set; } = new(PropertyFilter.Repairable);
    public SortOrderDirection Direction { get; set; } = SortOrderDirection.Ascending;
    public FillMode FillMode { get; set; } = FillMode.Top;
    public SortOrderMode SortMode { get; set; } = SortOrderMode.Alphabetically;
    public List<SortOrderMode> AdditionalSortModes { get; set; } = [];
    public List<AdditionalSortRule> AdditionalSortRules { get; set; } = [];
    public bool InclusiveAnd = false;

    public void ShowTooltip() {
        view.Draw();
    }

    public int Compare(InventorySlot? x, InventorySlot? y) {
        if (x is null) return 0;
        if (y is null) return 0;
        if (x.ExdItem.RowId is 0) return 0;
        if (y.ExdItem.RowId is 0) return 0;
        if (IsSortModeChainMatch(x.ExdItem, y.ExdItem)) return 0;
        if (CompareSlots(x, y)) return 1;
        return -1;
    }

    public bool IsItemSlotAllowed(InventorySlot slot) => InclusiveAnd ? 
        filterRules.Any(rule => rule.Active() && rule.IsSlotAllowed(slot)) : 
        filterRules.All(rule => !rule.Active() || rule.Active() && rule.IsSlotAllowed(slot));

    public bool CompareSlots(InventorySlot a, InventorySlot b) {
        var firstItem = a.ExdItem;
        var secondItem = b.ExdItem;

        switch (a.HasItem, b.HasItem) {
            // If both items are null, don't swap
            case (false, false): return false;

            // first slot empty, second slot full, if Ascending we want to leave justify, move the items left, if Descending right justify, leave the empty slot on the left.
            case (false, true): return FillMode is FillMode.Top;

            // first slot full, second slot empty, if Ascending we want to leave justify, and we have that already, if Descending right justify, move the item right
            case (true, false): return FillMode is FillMode.Bottom;

            case (true, true) when firstItem.RowId is not 0 && secondItem.RowId is not 0:
                
                var shouldSwap = false;
                
                // They are the same item
                if (firstItem.RowId == secondItem.RowId) {
                    // if left is not HQ, and right is HQ, swap
                    if (!a.InventoryItem->Flags.HasFlag(InventoryItem.ItemFlags.HighQuality) && b.InventoryItem->Flags.HasFlag(InventoryItem.ItemFlags.HighQuality)) {
                        shouldSwap = true;
                    }
                    // else if left has lower quantity then right, swap
                    else if (a.InventoryItem->Quantity < b.InventoryItem->Quantity) {
                        shouldSwap = true;
                    }
                }
                else {
                    shouldSwap = ShouldSwapBySortChain(firstItem, secondItem);
                }

                return shouldSwap;

            // Something went horribly wrong... best not touch it and walk away.
            default: return false;
        }
    }
    
    private bool IsSortModeChainMatch(Item firstItem, Item secondItem) {
        foreach (var sortRule in GetSortModeChain()) {
            if (!IsSortMatch(firstItem, secondItem, sortRule.Mode)) return false;
        }

        return true;
    }

    private IEnumerable<AdditionalSortRule> GetSortModeChain() {
        yield return new AdditionalSortRule {
            Mode = SortMode,
            Direction = Direction
        };

        MigrateLegacyAdditionalSortModes();
        foreach (var sortRule in AdditionalSortRules) {
            yield return sortRule;
        }
    }

    private bool ShouldSwapBySortChain(Item firstItem, Item secondItem) {
        foreach (var sortRule in GetSortModeChain()) {
            if (!IsSortMatch(firstItem, secondItem, sortRule.Mode)) {
                var shouldSwap = ShouldSwap(firstItem, secondItem, sortRule.Mode);
                return sortRule.Direction is SortOrderDirection.Descending ? !shouldSwap : shouldSwap;
            }
        }

        var shouldSwapFallback = ShouldSwap(firstItem, secondItem, SortOrderMode.Alphabetically);
        return Direction is SortOrderDirection.Descending ? !shouldSwapFallback : shouldSwapFallback;
    }

    /*
     * Backwards compatibility for older configs that only stored AdditionalSortModes.
     * New builds store AdditionalSortRules (mode + per-mode direction). When legacy values
     * are present and no new-format rules exist, migrate and clear the legacy list.
     */
    public void MigrateLegacyAdditionalSortModes() {
        if (AdditionalSortModes.Count is 0) return;

        if (AdditionalSortRules.Count is not 0) {
            AdditionalSortModes.Clear();
            return;
        }

        foreach (var sortMode in AdditionalSortModes) {
            AdditionalSortRules.Add(new AdditionalSortRule {
                Mode = sortMode,
                Direction = Direction
            });
        }

        AdditionalSortModes.Clear();
    }

    private static bool IsSortMatch(Item firstItem, Item secondItem, SortOrderMode sortMode) => sortMode switch {
        SortOrderMode.ItemId => firstItem.RowId == secondItem.RowId,
        SortOrderMode.ItemLevel => firstItem.LevelItem.RowId == secondItem.LevelItem.RowId,
        SortOrderMode.Alphabetically => string.Equals(firstItem.Name.ExtractText(), secondItem.Name.ExtractText(), StringComparison.OrdinalIgnoreCase),
        SortOrderMode.SellPrice => firstItem.PriceLow == secondItem.PriceLow,
        SortOrderMode.Rarity => firstItem.Rarity == secondItem.Rarity,
        SortOrderMode.ItemType => firstItem.ItemUICategory.RowId == secondItem.ItemUICategory.RowId,
        SortOrderMode.Level => firstItem.LevelEquip == secondItem.LevelEquip,
        _ => false,
    };

    private static bool ShouldSwap(Item firstItem, Item secondItem, SortOrderMode sortMode) => sortMode switch {
        SortOrderMode.ItemId => firstItem.RowId > secondItem.RowId,
        SortOrderMode.ItemLevel => firstItem.LevelItem.RowId > secondItem.LevelItem.RowId,
        SortOrderMode.Alphabetically => string.Compare(firstItem.Name.ExtractText(), secondItem.Name.ExtractText(), StringComparison.OrdinalIgnoreCase) > 0,
        SortOrderMode.SellPrice => firstItem.PriceLow > secondItem.PriceLow,
        SortOrderMode.Rarity => firstItem.Rarity > secondItem.Rarity,
        SortOrderMode.ItemType => ShouldSwapItemUiCategory(firstItem, secondItem),
        SortOrderMode.Level => firstItem.LevelEquip > secondItem.LevelEquip,
        _ => false,
    };

    private static bool ShouldSwapItemUiCategory(Item firstItem, Item secondItem) {
        // If same category, don't swap, other system handles fallback to alphabetical in this case
        if (firstItem.ItemUICategory.RowId == secondItem.ItemUICategory.RowId) return false;

        if (firstItem is { RowId: not 0, ItemUICategory.Value: var first } && secondItem is { RowId: not 0, ItemUICategory.Value: var second }) {
            if (first.OrderMajor == second.OrderMajor) {
                return first.OrderMinor > second.OrderMinor;
            }

            return first.OrderMajor > second.OrderMajor;
        }

        return false;
    }
}

public enum FillMode {
    [Description("Top")] 
    Top,

    [Description("Bottom")] 
    Bottom,
}

public enum ItemRarity {
    [Description("White")] 
    White = 1,

    [Description("Green")] 
    Green = 2,

    [Description("Blue")] 
    Blue = 3,

    [Description("Purple")] 
    Purple = 4,

    [Description("Pink")] 
    Pink = 7,
}

public enum SortOrderDirection {
    [Description("Ascending")] 
    Ascending,

    [Description("Descending")] 
    Descending,
}

public enum SortOrderMode {
    [Description("Alphabetical")] 
    Alphabetically,
    
    [Description("Item Level")] 
    ItemLevel,

    [Description("Rarity")] 
    Rarity,

    [Description("Sell Price")] 
    SellPrice,

    [Description("Item Id")] 
    ItemId,
    
    [Description("Item Type")]
    ItemType,
    
    [Description("Level")]
    Level,
}

public class SortingFilter {
    public required Func<bool> Active { get; init; }
    
    public required Func<InventorySlot, bool> IsSlotAllowed { get; init; }
}

public class AdditionalSortRule {
    public SortOrderMode Mode { get; set; } = SortOrderMode.ItemId;
    public SortOrderDirection Direction { get; set; } = SortOrderDirection.Ascending;
}