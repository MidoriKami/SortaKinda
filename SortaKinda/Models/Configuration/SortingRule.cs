using System.Numerics;
using SortaKinda.Interfaces;
using SortaKinda.System;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.Models;

public class SortingRule : ISortingRule
{
    public Vector4 Color { get; set; }
    public string Id { get; set; } = SortController.DefaultId;
    public string Name { get; set; } = "New Rule";
    public int Index { get; set; }

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