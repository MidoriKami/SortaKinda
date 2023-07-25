using System;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Interfaces;

namespace SortaKinda.Views.Tabs;

public class SortOrderTab : ITwoColumnRuleConfigurationTab
{
    public string TabName => "Sort Order";
    public bool Enabled => true;
    public ISortingRule SortingRule { get; set; }
    public string FirstLabel => "Sort By";
    public string SecondLabel => "Sort Options";

    public SortOrderTab(ISortingRule rule)
    {
        SortingRule = rule;
    }
    
    public void DrawLeftSideContents()
    {
        ImGui.Text("Order items using");
        ImGuiComponents.HelpMarker("The primary property of an item to use for ordering");
        var sortMode = SortingRule.SortMode;
        if (DrawRadioEnum(ref sortMode)) SortingRule.SortMode = sortMode;
    }
    
    public void DrawRightSideContents()
    {
        ImGui.Text("Sort item by");
        ImGuiComponents.HelpMarker("Ascending: A -> Z\nDescending Z -> A");
        var sortDirection = SortingRule.Direction;
        if (DrawRadioEnum(ref sortDirection)) SortingRule.Direction = sortDirection;
        
        ImGuiHelpers.ScaledDummy(8.0f);
        ImGui.Text("Fill inventory slots from");
        ImGuiComponents.HelpMarker("Top - Items are shifted to the top left-most slots\nBottom - Items are shifted to the bottom right-most slots");
        var fillMode = SortingRule.FillMode;
        if (DrawRadioEnum(ref fillMode)) SortingRule.FillMode = fillMode;
    }
    
    private static bool DrawRadioEnum<T>(ref T configValue) where T : Enum
    {
        foreach (Enum mode in Enum.GetValues(configValue.GetType()))
        {
            var isSelected = Convert.ToInt32(configValue);
            if (ImGui.RadioButton(mode.GetLabel(), ref isSelected, Convert.ToInt32(mode)))
            {
                configValue = (T) mode;
                return true;
            }
        }

        return false;
    }
}