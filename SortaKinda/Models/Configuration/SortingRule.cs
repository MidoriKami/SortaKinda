﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.Models.Enums;
using SortaKinda.Models.General;
using SortaKinda.System;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.Models;

public unsafe class SortingRule : ISortingRule
{
    private readonly SortingRuleTooltipView view;

    public SortingRule()
    {
        view = new SortingRuleTooltipView(this);
    }

    public Vector4 Color { get; set; }
    public string Id { get; set; } = SortController.DefaultId;
    public string Name { get; set; } = "New Rule";
    public int Index { get; set; }
    public HashSet<string> AllowedItemNames { get; set; } = new();
    public HashSet<uint> AllowedItemTypes { get; set; } = new();
    public HashSet<ItemRarity> AllowedItemRarities { get; set; } = new();
    public RangeFilter ItemLevelFilter { get; set; } = new("Item Level Filter", 0, 1000);
    public RangeFilter VendorPriceFilter { get; set; } = new("Vendor Price Filter", 0, 1_000_000);
    public ToggleFilter UntradableFilter { get; set; } = new(PropertyFilter.Untradable);
    public ToggleFilter UniqueFilter { get; set; } = new(PropertyFilter.Unique);
    public ToggleFilter CollectableFilter { get; set; } = new(PropertyFilter.Collectable);
    public ToggleFilter DyeableFilter { get; set; } = new(PropertyFilter.Dyeable);
    public SortOrderDirection Direction { get; set; } = SortOrderDirection.Ascending;
    public FillMode FillMode { get; set; } = FillMode.Top;
    public SortOrderMode SortMode { get; set; } = SortOrderMode.Alphabetically;

    public void ShowTooltip()
    {
        view.Draw();
    }

    public int Compare(IInventorySlot? x, IInventorySlot? y)
    {
        if (x is null) return 0;
        if (y is null) return 0;
        if (x.ExdItem is null) return 0;
        if (y.ExdItem is null) return 0;
        if (IsFilterMatch(x.ExdItem, y.ExdItem)) return 0;
        if (CompareSlots(x, y)) return 1;
        return -1;
    }

    public bool IsItemSlotAllowed(IInventorySlot slot)
    {
        if (AllowedItemNames.Count > 0 && !AllowedItemNames.Any(allowed => Regex.IsMatch(slot.ExdItem?.Name.RawString ?? string.Empty, allowed, RegexOptions.IgnoreCase))) return false;
        if (AllowedItemTypes.Count > 0 && !AllowedItemTypes.Any(allowed => slot.ExdItem?.ItemUICategory.Row == allowed)) return false;
        if (AllowedItemRarities.Count > 0 && !AllowedItemRarities.Any(allowed => slot.ExdItem?.Rarity == (byte) allowed)) return false;
        if (ItemLevelFilter.Enable && (slot.ExdItem?.LevelItem.Row > ItemLevelFilter.MaxValue || slot.ExdItem?.LevelItem.Row < ItemLevelFilter.MinValue)) return false;
        if (VendorPriceFilter.Enable && (slot.ExdItem?.PriceLow > VendorPriceFilter.MaxValue || slot.ExdItem?.PriceLow < VendorPriceFilter.MinValue)) return false;
        if (!UntradableFilter.IsItemSlotAllowed(slot)) return false;
        if (!UniqueFilter.IsItemSlotAllowed(slot)) return false;
        if (!CollectableFilter.IsItemSlotAllowed(slot)) return false;
        if (!DyeableFilter.IsItemSlotAllowed(slot)) return false;

        return true;
    }

    public bool CompareSlots(IInventorySlot a, IInventorySlot b)
    {
        var firstItem = a.ExdItem;
        var secondItem = b.ExdItem;

        switch (a.HasItem, b.HasItem)
        {
            // If both items are null, don't swap
            case (false, false): return false;

            // first slot empty, second slot full, if Ascending we want to left justify, move the items left, if Descending right justify, leave the empty slot on the left.
            case (false, true): return FillMode is FillMode.Top;

            // first slot full, second slot empty, if Ascending we want to left justify, and we have that already, if Descending right justify, move the item right
            case (true, false): return FillMode is FillMode.Bottom;

            case (true, true) when firstItem is not null && secondItem is not null:
                
                var shouldSwap = false;
                
                // They are the same item
                if (firstItem.RowId == secondItem.RowId)
                {
                    // if left is not HQ, and right is HQ, swap
                    if (!a.InventoryItem->Flags.HasFlag(InventoryItem.ItemFlags.HQ) && b.InventoryItem->Flags.HasFlag(InventoryItem.ItemFlags.HQ))
                    {
                        shouldSwap = true;
                    }
                    // else if left has lower quantity then right, swap
                    else if (a.InventoryItem->Quantity < b.InventoryItem->Quantity)
                    {
                        shouldSwap = true;
                    }
                }
                // else if they match according to the default filter, fallback to alphabetical
                else if (IsFilterMatch(firstItem, secondItem))
                {
                    shouldSwap = ShouldSwap(firstItem, secondItem, SortOrderMode.Alphabetically);
                }
                // else they are not the same item, and the filter result doesn't match
                else
                {
                    shouldSwap = ShouldSwap(firstItem, secondItem, SortMode);
                }
                
                return Direction is SortOrderDirection.Descending ? !shouldSwap : shouldSwap;

            // Something went horribly wrong... best not touch it and walk away.
            default: return false;
        }
    }
    
    private bool IsFilterMatch(Item firstItem, Item secondItem) => SortMode switch
    {
        SortOrderMode.ItemId => firstItem.RowId == secondItem.RowId,
        SortOrderMode.ItemLevel => firstItem.LevelItem.Row == secondItem.LevelItem.Row,
        SortOrderMode.Alphabetically => string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) == 0,
        SortOrderMode.SellPrice => firstItem.PriceLow == secondItem.PriceLow,
        SortOrderMode.Rarity => firstItem.Rarity == secondItem.Rarity,
        SortOrderMode.ItemType => ItemTypeSwap(firstItem, secondItem) == 0,
        _ => false
    };

    private static int ItemTypeSwap(Item firstItem, Item secondItem)
    {
        switch (firstItem.ItemSortCategory.Value!.Param.CompareTo(secondItem.ItemSortCategory.Value!.Param))
        {
            case < 0: return -1;
            case > 0: return 1;
            default: break;
        }

        switch (ShouldSwapItemUiCategory(firstItem, secondItem))
        {
            case < 0: return -1;
            case > 0: return 1;
            default: break;
        }

        switch (firstItem.Unknown19.CompareTo(secondItem.Unknown19))
        {
            case < 0: return -1;
            case > 0: return 1;
            default: break;
        }

        switch (firstItem.RowId.CompareTo(secondItem.RowId))
        {
            case < 0: return -1;
            case > 0: return 1;
            default: break;
        }

        return 0;
    }

    private static bool ShouldSwap(Item firstItem, Item secondItem, SortOrderMode sortMode) => sortMode switch
    {
        SortOrderMode.ItemId => firstItem.RowId > secondItem.RowId,
        SortOrderMode.ItemLevel => firstItem.LevelItem.Row > secondItem.LevelItem.Row,
        SortOrderMode.Alphabetically => string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) > 0,
        SortOrderMode.SellPrice => firstItem.PriceLow > secondItem.PriceLow,
        SortOrderMode.Rarity => firstItem.Rarity > secondItem.Rarity,
        SortOrderMode.ItemType => ItemTypeSwap(firstItem, secondItem) > 0,
        _ => false
    };

    private static int ShouldSwapItemUiCategory(Item firstItem, Item secondItem)
    {
        if (firstItem is { ItemUICategory.Value: { } first } && secondItem is { ItemUICategory.Value: { } second })
        {
            switch (first.OrderMajor.CompareTo(second.OrderMajor))
            {
                case < 0: return -1;
                case > 0: return 1;
                default: break;
            }

            switch (first.OrderMinor.CompareTo(second.OrderMinor))
            {
                case < 0: return -1;
                case > 0: return 1;
                default: break;
            }
        }

        return 0;
    }
}