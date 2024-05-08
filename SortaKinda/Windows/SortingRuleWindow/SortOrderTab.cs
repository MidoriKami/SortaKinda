using System;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Classes;
using SortaKinda.Configuration;
using SortaKinda.Windows.SortingRuleWindow.Components;

namespace SortaKinda.Windows.SortingRuleWindow;

public class SortOrderTab(SortingRule rule) : ITwoColumnRuleConfigurationTab {
    public string Name => "Sort Order";
    
    public bool Disabled => false;
    
    public SortingRule SortingRule { get; } = rule;
    
    public string FirstLabel => "Sort By";
    
    public string SecondLabel => "Sort Options";

    public void DrawLeftSideContents() {
        ImGui.Text("Order items using");
        ImGuiComponents.HelpMarker("The primary property of an item to use for ordering");
        var sortMode = SortingRule.SortMode;
        DrawRadioEnum(ref sortMode);

        SortingRule.SortMode = sortMode;
    }

    public void DrawRightSideContents() {
        ImGui.Text("Sort item by");
        ImGuiComponents.HelpMarker("Ascending: A -> Z\nDescending Z -> A");
        var sortDirection = SortingRule.Direction;
        DrawRadioEnum(ref sortDirection);

        ImGuiHelpers.ScaledDummy(8.0f);
        ImGui.Text("Fill inventory slots from");
        ImGuiComponents.HelpMarker("Top - Items are shifted to the top left-most slots\nBottom - Items are shifted to the bottom right-most slots");
        var fillMode = SortingRule.FillMode;
        DrawRadioEnum(ref fillMode);

        SortingRule.Direction = sortDirection;
        SortingRule.FillMode = fillMode;
    }

    private static void DrawRadioEnum<T>(ref T configValue) where T : Enum {
        foreach (Enum value in Enum.GetValues(configValue.GetType())) {
            var isSelected = Convert.ToInt32(configValue);
            if (ImGui.RadioButton($"{value.GetDescription()}##{configValue.GetType()}", ref isSelected, Convert.ToInt32(value))) {
                configValue = (T) value;
            }
        }
    }
}