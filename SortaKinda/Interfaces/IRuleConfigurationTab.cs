using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Interfaces;
using SortaKinda.System;

namespace SortaKinda.Interfaces;

public interface IRuleConfigurationTab : ITabItem
{
    private static Vector2 FooterSize => ImGuiHelpers.ScaledVector2(0.0f, 30.0f);
    protected ISortingRule SortingRule { get; }

    void ITabItem.Draw()
    {
        if (ImGui.BeginChild("##RuleConfigurationTabChild", ImGui.GetContentRegionAvail() - FooterSize - ImGui.GetStyle().FramePadding))
        {
            DrawConfigurationTab();
        }
        ImGui.EndChild();

        if (ImGui.BeginChild("##RuleConfigurationTabFooter", FooterSize - ImGui.GetStyle().FramePadding))
        {
            DrawFooter();
        }
        ImGui.EndChild();
    }

    void DrawConfigurationTab();

    void DrawFooter()
    {
        var buttonSize = ImGuiHelpers.ScaledVector2(100.0f, 23.0f);

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3.0f * ImGuiHelpers.GlobalScale);
        ImGui.TextColored(KnownColor.Gray.Vector(), SortingRule.Id);

        ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonSize.X * 2.0f - ImGui.GetStyle().ItemSpacing.X);
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Save", buttonSize))
        {
            SortaKindaController.ModuleController.Sort();
            SortaKindaController.SortController.SaveConfig();
        }

        ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Save & Close", buttonSize))
        {
            SortaKindaController.ModuleController.Sort();
            SortaKindaController.SortController.SaveConfig();
            RuleConfigWindowController.RemoveRuleConfigWindow(SortingRule.Id);
        }
    }
}