using System.Drawing;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Interfaces;

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

        if (rule.Name is not "Unsorted")
        {
            ImGui.TextColored(KnownColor.Gray.AsVector4(), GetAllowedItemsString());
            ImGui.TextColored(KnownColor.Gray.AsVector4(), GetSortingModesString());
        }

        ImGui.EndTooltip();
    }
    
    private string GetAllowedItemsString()
    {
        return "Allowed Item Code Not Written";
    }
    
    private string GetSortingModesString()
    {
        return "Sorting Modes Code Not Written";
    }
}