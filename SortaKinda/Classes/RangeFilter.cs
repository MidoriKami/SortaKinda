using Dalamud.Interface.Utility;
using ImGuiNET;

namespace SortaKinda.Classes;

public class RangeFilter(string label, int minValue, int maxValue) {
    public bool Enable;
    public string Label = label;
    public int MaxValue = maxValue;
    public int MinValue = minValue;

    public void DrawConfig() {
        ImGui.TextUnformatted(Label);

        ImGui.Checkbox($"##Enable{Label}", ref Enable);
        ImGui.SameLine(0, ImGui.GetStyle().ItemInnerSpacing.X * 2.0f);
        
        ImGui.PushItemWidth(ImGui.GetContentRegionMax().X / 3.0f);
        ImGui.InputInt($"##Minimum{Label}", ref MinValue, 0, 0);

        ImGui.SameLine(0, ImGui.GetStyle().ItemInnerSpacing.X);
        ImGui.PushItemWidth(ImGui.GetContentRegionMax().X / 3.0f);
        ImGui.InputInt($"##Maximum{Label}", ref MaxValue, 0, 0);

        ImGuiHelpers.ScaledDummy(8.0f);
    }

    public bool IsItemSlotAllowed(uint? itemSlotValue) 
        => IsItemSlotAllowed((int?) itemSlotValue);
    
    public bool IsItemSlotAllowed(int? itemSlotValue) {
        if (itemSlotValue is null) return false;
        if (itemSlotValue < MinValue) return false;
        if (itemSlotValue > MaxValue) return false;

        return true;
    }
}