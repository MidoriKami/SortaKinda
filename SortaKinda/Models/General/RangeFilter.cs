using Dalamud.Interface;
using ImGuiNET;

namespace SortaKinda.Models;

public class RangeFilter
{
    public bool Enable;

    public string Label;
    public int MaxValue;
    public int MinValue;

    public RangeFilter(string label, int defaultMinimum, int defaultMaximum)
    {
        Label = label;
        MinValue = defaultMinimum;
        MaxValue = defaultMaximum;
    }

    public void DrawConfig()
    {
        ImGui.TextUnformatted(Label);

        ImGui.Checkbox($"##Enable{Label}", ref Enable);
        ImGui.SameLine();

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing);

        ImGui.PushItemWidth(ImGui.GetContentRegionMax().X / 3.0f);
        ImGui.InputInt($"##Minimum{Label}", ref MinValue, 0, 0);

        ImGui.SameLine();
        ImGui.PushItemWidth(ImGui.GetContentRegionMax().X / 3.0f);
        ImGui.InputInt($"##Maximum{Label}", ref MaxValue, 0, 0);

        ImGui.PopStyleVar();

        ImGuiHelpers.ScaledDummy(8.0f);
    }
}