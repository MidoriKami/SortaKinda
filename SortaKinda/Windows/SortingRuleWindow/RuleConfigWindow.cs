using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Style;
using Dalamud.Interface.Utility;
using ImGuiNET;
using SortaKinda.Models;
using SortaKinda.System;
using SortaKinda.Views.SortControllerViews;
using Window = KamiLib.Window.Window;

namespace SortaKinda.Views.Windows;

public class RuleConfigWindow : Window {
    private readonly List<SortingRule> ruleList;
    private readonly SortingRuleView view;
    private readonly SortingRule rule;
    private static Vector2 FooterSize => ImGuiHelpers.ScaledVector2(0.0f, 30.0f);

    public RuleConfigWindow(SortingRule sortingRule, List<SortingRule> sortingRules) 
        : base($"SortaKinda Rule Configuration - {sortingRule.Name}###RuleConfig{sortingRule.Id}", new Vector2(500.0f, 350.0f)) {
        rule = sortingRule;
        ruleList = sortingRules;
        view = new SortingRuleView(sortingRule);

        Position = ImGui.GetMainViewport().Size / 2.0f - new Vector2(500.0f, 400.0f) / 2.0f;
        PositionCondition = ImGuiCond.Appearing;

        IsOpen = true;
    }

    public override void PreDraw() 
        => StyleModelV1.DalamudStandard.Push();

    public override void Draw() {
        DrawHeader();
        
        if (ImGui.BeginChild("##RuleConfigurationTabChild", ImGui.GetContentRegionAvail() - FooterSize - ImGui.GetStyle().FramePadding)) {
            view.Draw();
        }
        ImGui.EndChild();
        
        if (ImGui.BeginChild("##RuleConfigurationTabFooter", FooterSize - ImGui.GetStyle().FramePadding)) {
            DrawFooter();
        }
        ImGui.EndChild();
        DrawPopupWindow();
    }
    
    public override void PostDraw() 
        => StyleModelV1.DalamudStandard.Pop();

    private void DrawHeader() {
        DrawColorEdit();
        DrawNameEdit();
        DrawDeleteButton();
        DrawAdvancedOptionsButton();
        ImGuiHelpers.ScaledDummy(5.0f);
    }

    private void DrawColorEdit() {
        var region = ImGui.GetContentRegionAvail();

        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X / 4.0f - ImGuiHelpers.GlobalScale * 50.0f + ImGui.GetStyle().ItemSpacing.X / 2.0f });
        var imGuiColor = rule.Color;
        if (ImGui.ColorEdit4("##ColorConfig", ref imGuiColor, ImGuiColorEditFlags.NoInputs)) {
            rule.Color = imGuiColor;
        }
    }

    private void DrawNameEdit() {
        var region = ImGui.GetContentRegionAvail();

        ImGui.SameLine();
        ImGui.SetNextItemWidth(region.X / 2.0f - ImGui.GetItemRectSize().X - ImGui.GetStyle().ItemSpacing.X);
        var imGuiName = rule.Name;
        if (ImGui.InputText("##NameEdit", ref imGuiName, 1024, ImGuiInputTextFlags.AutoSelectAll)) {
            rule.Name = imGuiName;
            WindowName = $"SortaKinda Rule Configuration - {rule.Name}###RuleConfig{rule.Id}";
        }
    }

    private void DrawDeleteButton() {
        var hotkeyHeld = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl;
        if (!hotkeyHeld) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
        ImGui.SameLine();
        if (ImGui.Button("Delete", ImGuiHelpers.ScaledVector2(100.0f, 23.0f)) && hotkeyHeld) {
            ruleList.Remove(rule);
            IsOpen = false;
        }
        if (!hotkeyHeld) ImGui.PopStyleVar();
        if (ImGui.IsItemHovered() && !hotkeyHeld) {
            ImGui.SetTooltip("Hold Shift + Control while clicking to delete this rule");
        }
    }

    private void DrawAdvancedOptionsButton() {
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - ImGui.GetFrameHeight());
        if (ImGuiComponents.IconButton("AdvancedOptions", FontAwesomeIcon.Cog)) {
            ImGui.OpenPopup("Advanced Options");
        }
    }

    private void DrawPopupWindow() {
        ImGui.SetNextWindowSize(new Vector2(200.0f, 200.0f), ImGuiCond.Always);
        if (ImGui.BeginPopup("Advanced Options")) {
            if (ImGui.Checkbox("Use Inclusive Logic", ref rule.InclusiveAnd)) {
                SortaKindaController.SortController.SaveConfig();
            }
            
            ImGui.EndPopup();
        }
    }

    public override void OnClose() {
        SortaKindaController.SortController.SaveConfig();
        SortaKindaController.WindowManager.RemoveWindow(this);
    }
    
    private void DrawFooter() {
        var buttonSize = ImGuiHelpers.ScaledVector2(100.0f, 23.0f);

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3.0f * ImGuiHelpers.GlobalScale);
        ImGui.TextColored(KnownColor.Gray.Vector(), rule.Id);

        ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonSize.X * 2.0f - ImGui.GetStyle().ItemSpacing.X);
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Save", buttonSize)) {
            SortaKindaController.ModuleController.Sort();
            SortaKindaController.SortController.SaveConfig();
        }

        ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Save & Close", buttonSize)) {
            SortaKindaController.ModuleController.Sort();
            SortaKindaController.SortController.SaveConfig();
            Close();
        }
    }
}