using System.Collections.Generic;
using System.Numerics;
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
}