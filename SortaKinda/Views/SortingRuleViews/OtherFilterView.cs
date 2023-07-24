using System;
using System.Collections.Generic;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Models.Enum;

namespace SortaKinda.Views.SortingRuleViews;

public class OtherFilterView
{
    public static void DrawItemLevelFilter(ref bool enable, ref int minItemLevel, ref int maxItemLevel)
    {
        ImGui.Checkbox("##ItemLevelRangeEnable", ref enable);
        ImGui.SameLine();
       
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing);

        ImGui.PushItemWidth(ImGui.GetContentRegionMax().X / 3.0f);
        ImGui.InputInt("##MinItemLevel", ref minItemLevel, 0, 0);
        
        ImGui.SameLine();
        ImGui.PushItemWidth(ImGui.GetContentRegionMax().X / 3.0f);
        ImGui.InputInt("##MaxItemLevel", ref maxItemLevel, 0, 0);
        
        ImGui.PopStyleVar();
    }

    public static void DrawSalePriceFilter(ref bool enable, ref int minPrice, ref int maxPrice)
    {
        ImGui.TextUnformatted("Vendor Price Range");
        ImGui.Separator();

        ImGui.Checkbox("##PriceRangeEnable", ref enable);
        ImGui.SameLine();
      
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing);

        ImGui.PushItemWidth(ImGui.GetContentRegionMax().X / 3.0f);
        ImGui.InputInt("##MinPrice", ref minPrice, 0, 0);
        
        ImGui.SameLine();
        ImGui.PushItemWidth(ImGui.GetContentRegionMax().X / 3.0f);
        ImGui.InputInt("##MaxPrice", ref maxPrice, 0, 0);
        
        ImGui.PopStyleVar();
    }

    public static void DrawRarityFilter(HashSet<ItemRarity> itemRarities)
    {
        foreach (var enumValue in Enum.GetValues<ItemRarity>())
        {
            var enabled = itemRarities.Contains(enumValue);
            if (ImGuiComponents.ToggleButton($"{enumValue.GetLabel()}", ref enabled))
            {
                if (enabled) itemRarities.Add(enumValue);
                if (!enabled) itemRarities.Remove(enumValue);
            }
            
            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3.0f);
            ImGui.TextUnformatted(enumValue.GetLabel());
        }
    }
}