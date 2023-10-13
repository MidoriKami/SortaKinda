using System;
using System.Drawing;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Game;
using KamiLib.Utility;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.System;

namespace SortaKinda.Views.SortControllerViews;

public class SortingRuleTooltipView
{
    private readonly ISortingRule rule;

    public SortingRuleTooltipView(ISortingRule sortingRule)
    {
        rule = sortingRule;
    }

    public void Draw()
    {
        ImGui.BeginTooltip();

        var imGuiColor = rule.Color;
        if (ImGui.ColorEdit4("##ColorTooltip", ref imGuiColor, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoPicker))
        {
            rule.Color = imGuiColor;
        }

        ImGui.SameLine();
        ImGui.Text(rule.Name);

        if (rule.Id is not SortController.DefaultId)
        {
            var itemFiltersString = GetAllowedItemsString();

            ImGui.TextColored(KnownColor.Gray.Vector(), itemFiltersString.IsNullOrEmpty() ? "Any Item" : itemFiltersString);
            ImGui.TextColored(KnownColor.Gray.Vector(), rule.SortMode.Label());
        }

        ImGui.EndTooltip();
    }

    private string GetAllowedItemsString()
    {
        var strings = new[]
        {
            rule.AllowedItemTypes.Count > 0 ? string.Join(", ", rule.AllowedItemTypes.Select(type => LuminaCache<ItemUICategory>.Instance.GetRow(type)?.Name.RawString)) : string.Empty,
            rule.AllowedItemNames.Count > 0 ? string.Join(", ", rule.AllowedItemNames.Select(name => @$"""{name}""")) : string.Empty,
            rule.AllowedItemRarities.Count > 0 ? string.Join(", ", rule.AllowedItemRarities.Select(rarity => rarity.Label())) : string.Empty,
            rule.ItemLevelFilter.Enable ? $"{rule.ItemLevelFilter.MinValue} ilvl → {rule.ItemLevelFilter.MaxValue} ilvl" : string.Empty,
            rule.VendorPriceFilter.Enable ? $"{rule.VendorPriceFilter.MinValue} gil → {rule.VendorPriceFilter.MaxValue} gil" : string.Empty
        };

        return string.Join("\n", strings
            .Where(eachString => !eachString.IsNullOrEmpty())
            .Select(eachString => eachString[..Math.Min(eachString.Length, 55)]));
    }
}