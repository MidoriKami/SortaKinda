using System;
using System.Drawing;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Classes;
using SortaKinda.Controllers;

namespace SortaKinda.ViewComponents;

public class SortingRuleTooltipView(SortingRule sortingRule) {
    public void Draw() {
        ImGui.BeginTooltip();

        var imGuiColor = sortingRule.Color;
        if (ImGui.ColorEdit4("##ColorTooltip", ref imGuiColor, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoPicker)) {
            sortingRule.Color = imGuiColor;
        }

        ImGui.SameLine();
        ImGui.Text(sortingRule.Name);

        if (sortingRule.Id is not SortController.DefaultId) {
            var itemFiltersString = GetAllowedItemsString();

            ImGui.TextColored(KnownColor.Gray.Vector(), itemFiltersString.IsNullOrEmpty() ? "Any Item" : itemFiltersString);
            ImGui.TextColored(KnownColor.Gray.Vector(), sortingRule.SortMode.GetDescription());
        }

        ImGui.EndTooltip();
    }

    private string GetAllowedItemsString() {
        var strings = new[] {
            sortingRule.AllowedItemTypes.Count > 0 ? string.Join(", ", sortingRule.AllowedItemTypes.Select(type => Service.DataManager.GetExcelSheet<ItemUICategory>()!.GetRow(type)?.Name.RawString)) : string.Empty,
            sortingRule.AllowedNameRegexes.Count > 0 ? string.Join(", ", sortingRule.AllowedNameRegexes.Select(regex => @$"""{regex.Text}""")) : string.Empty,
            sortingRule.AllowedItemRarities.Count > 0 ? string.Join(", ", sortingRule.AllowedItemRarities.Select(rarity => rarity.GetDescription())) : string.Empty,
            sortingRule.ItemLevelFilter.Enable ? $"{sortingRule.ItemLevelFilter.MinValue} ilvl → {sortingRule.ItemLevelFilter.MaxValue} ilvl" : string.Empty,
            sortingRule.VendorPriceFilter.Enable ? $"{sortingRule.VendorPriceFilter.MinValue} gil → {sortingRule.VendorPriceFilter.MaxValue} gil" : string.Empty,
            sortingRule.LevelFilter.Enable ? $"{sortingRule.LevelFilter.MinValue} lvl → {sortingRule.LevelFilter.MaxValue} lvl" : string.Empty,
        };

        return string.Join("\n", strings
            .Where(eachString => !eachString.IsNullOrEmpty())
            .Select(eachString => eachString[..Math.Min(eachString.Length, 55)]));
    }
}