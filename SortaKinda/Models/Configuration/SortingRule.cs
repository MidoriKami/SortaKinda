﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.Models.Enum;
using SortaKinda.System;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.Models;

public class SortingRule : ISortingRule
{
    public Vector4 Color { get; set; }
    public string Id { get; set; } = SortController.DefaultId;
    public string Name { get; set; } = "New Rule";
    public int Index { get; set; }
    public HashSet<string> AllowedItemNames { get; set; } = new();
    public HashSet<uint> AllowedItemTypes { get; set; } = new();
    public HashSet<ItemRarity> AllowedItemRarities { get; set; } = new();
    public RangeFilter ItemLevelFilter { get; set; } = new("Item Level Filter", 0, 1000);
    public RangeFilter VendorPriceFilter { get; set; } = new("Vendor Price Filter", 0, 1_000_000);
    public SortOrderDirection Direction { get; set; } = SortOrderDirection.Ascending;
    public FillMode FillMode { get; set; } = FillMode.Top;
    public SortOrderMode SortMode { get; set; } = SortOrderMode.Alphabetically;

    private readonly SortingRuleTooltipView view;

    public SortingRule()
    {
        view = new SortingRuleTooltipView(this);
    }
    
    public void ShowTooltip()
    {
        view.Draw();
    }
    
    public int Compare(IInventorySlot? x, IInventorySlot? y)
    {
        if (x is null) return 0;
        if (y is null) return 0;
        if (x.Item is null) return 0;
        if (y.Item is null) return 0;
        if (IsItemMatch(x.Item, y.Item)) return 0;
        if (CompareSlots(x, y)) return 1;
        return -1;
    }

    public bool CompareSlots(IInventorySlot a, IInventorySlot b)
    {
        var firstItem = a.Item;
        var secondItem = b.Item;

        switch (a.HasItem, b.HasItem)
        {
            // If both items are null, don't swap
            case (false, false): return false;

            // first slot empty, second slot full, if Ascending we want to left justify, move the items left, if Descending right justify, leave the empty slot on the left.
            case (false, true): return FillMode is FillMode.Top;

            // first slot full, second slot empty, if Ascending we want to left justify, and we have that already, if Descending right justify, move the item right
            case (true, false): return FillMode is FillMode.Bottom;

            case (true, true) when firstItem is not null && secondItem is not null:
                var shouldSwap = ShouldSwap(firstItem, secondItem, IsItemMatch(firstItem, secondItem) ? SortOrderMode.Alphabetically : SortMode);

                if (Direction is SortOrderDirection.Descending)
                    shouldSwap = !shouldSwap;

                return shouldSwap;

            // Something went horribly wrong... best not touch it and walk away.
            default: return false;
        }
    }
    
    private bool IsItemMatch(Item firstItem, Item secondItem) => SortMode switch
    {
        SortOrderMode.ItemId => firstItem.RowId == secondItem.RowId,
        SortOrderMode.ItemLevel => firstItem.LevelItem.Row == secondItem.LevelItem.Row,
        SortOrderMode.Alphabetically => string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) == 0,
        SortOrderMode.SellPrice => firstItem.PriceLow == secondItem.PriceLow,
        SortOrderMode.Rarity => firstItem.Rarity == secondItem.Rarity,
        _ => false
    };

    private static bool ShouldSwap(Item firstItem, Item secondItem, SortOrderMode sortMode) => sortMode switch
    {
        SortOrderMode.ItemId => firstItem.RowId > secondItem.RowId,
        SortOrderMode.ItemLevel => firstItem.LevelItem.Row > secondItem.LevelItem.Row,
        SortOrderMode.Alphabetically => string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) > 0,
        SortOrderMode.SellPrice => firstItem.PriceLow > secondItem.PriceLow,
        SortOrderMode.Rarity => firstItem.Rarity > secondItem.Rarity,
        _ => false
    };
    
    public bool IsItemSlotAllowed(IInventorySlot slot)
    {
        if (AllowedItemNames.Count > 0 && !AllowedItemNames.Any(allowed => Regex.IsMatch(slot.Item?.Name.RawString ?? string.Empty, allowed, RegexOptions.IgnoreCase))) return false;
        if (AllowedItemTypes.Count > 0 && !AllowedItemTypes.Any(allowed => slot.Item?.ItemUICategory.Row == allowed)) return false;
        if (AllowedItemRarities.Count > 0 && !AllowedItemRarities.Any(allowed => slot.Item?.Rarity == (byte)allowed)) return false;
        if (ItemLevelFilter.Enable && (slot.Item?.LevelItem.Row > ItemLevelFilter.MaxValue || slot.Item?.LevelItem.Row < ItemLevelFilter.MinValue)) return false;
        if (VendorPriceFilter.Enable && (slot.Item?.PriceLow > VendorPriceFilter.MaxValue || slot.Item?.PriceLow < VendorPriceFilter.MinValue)) return false;

        return true;
    }
}