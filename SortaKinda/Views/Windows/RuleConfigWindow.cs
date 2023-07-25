using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using SortaKinda.Models;
using SortaKinda.System;

namespace SortaKinda.Views.Windows;

public class RuleConfigWindow : Window
{
    public SortingRule Rule;
    private readonly List<SortingRule> ruleList;

    public RuleConfigWindow(SortingRule sortingRule, List<SortingRule> sortingRules) : base($"SortaKinda Rule Configuration - {sortingRule.Name}###{sortingRule.Id}")
    {
        Rule = sortingRule;
        ruleList = sortingRules;

        Position = ImGui.GetMainViewport().Size / 2.0f - new Vector2(500.0f, 400.0f) / 2.0f;
        PositionCondition = ImGuiCond.Appearing;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500.0f, 400.0f),
            MaximumSize = new Vector2(9999, 9999)
        };
        
        IsOpen = true;
    }

    public override void Draw()
    {
        DrawHeader();
    }
    
    private void DrawHeader()
    {
        DrawColorEdit();
        DrawNameEdit();
        DrawDeleteButton();
    }
    
    private void DrawColorEdit()
    {
        var region = ImGui.GetContentRegionAvail();
        
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X / 4.0f - ImGuiHelpers.GlobalScale * 50.0f + ImGui.GetStyle().ItemSpacing.X / 2.0f });
        var imGuiColor = Rule.Color;
        if (ImGui.ColorEdit4("##ColorConfig", ref imGuiColor, ImGuiColorEditFlags.NoInputs))
        {
            Rule.Color = imGuiColor;
        }
    }
    
    private void DrawNameEdit()
    {
        var region = ImGui.GetContentRegionAvail();

        ImGui.SameLine();
        ImGui.SetNextItemWidth(region.X / 2.0f - ImGui.GetItemRectSize().X - ImGui.GetStyle().ItemSpacing.X);
        var imGuiName = Rule.Name;
        if (ImGui.InputText("##NameEdit", ref imGuiName, 1024, ImGuiInputTextFlags.AutoSelectAll))
        {
            Rule.Name = imGuiName;
            WindowName = $"SortaKinda Rule Configuration - {Rule.Name}###{Rule.Id}";
        }
    }
    
    private void DrawDeleteButton()
    {
        var hotkeyHeld = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyAlt;
        if (!hotkeyHeld) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
        ImGui.SameLine();
        if (ImGui.Button("Delete", ImGuiHelpers.ScaledVector2(100.0f, 23.0f)) && hotkeyHeld)
        {
            ruleList.Remove(Rule);
            IsOpen = false;
        }
        if (!hotkeyHeld) ImGui.PopStyleVar();
        if (ImGui.IsItemHovered() && !hotkeyHeld)
        {
            ImGui.SetTooltip("Hold Shift + Alt while clicking to delete this rule");
        }
        ImGuiHelpers.ScaledDummy(5.0f);
    }

    public override void OnClose()
    {
        SortaKindaController.SortController.SaveConfig();
        SortaKindaController.RuleConfigController.RemoveRuleConfigWindow(this);
    }
}