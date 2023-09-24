using System;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Interfaces;
using SortaKinda.Models.Enums;

namespace SortaKinda.Views.Tabs;

public class OtherFiltersTab : ITwoColumnRuleConfigurationTab
{
    public OtherFiltersTab(ISortingRule rule)
    {
        SortingRule = rule;
    }

    public string TabName => "Other Filters";
    public bool Enabled => true;
    public ISortingRule SortingRule { get; }
    public string FirstLabel => "Range Filters";
    public string SecondLabel => "Item Rarity Filter";

    public void DrawLeftSideContents()
    {
        SortingRule.ItemLevelFilter.DrawConfig();
        SortingRule.VendorPriceFilter.DrawConfig();
    }

    public void DrawRightSideContents()
    {
        foreach (var enumValue in Enum.GetValues<ItemRarity>())
        {
            var enabled = SortingRule.AllowedItemRarities.Contains(enumValue);
            if (ImGuiComponents.ToggleButton($"{enumValue.Label()}", ref enabled))
            {
                if (enabled) SortingRule.AllowedItemRarities.Add(enumValue);
                if (!enabled) SortingRule.AllowedItemRarities.Remove(enumValue);
            }

            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3.0f);
            ImGui.TextUnformatted(enumValue.Label());
        }
    }
}