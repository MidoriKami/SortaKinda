using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Style;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
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
        
        TitleBarButtons.Add(new TitleBarButton {
            Icon = FontAwesomeIcon.Cog,
            ShowTooltip = () => ImGui.SetTooltip("Additional Settings"),
            IconOffset = new Vector2(2.0f, 2.0f),
            Click = _ => ImGui.OpenPopup("Advanced Options")
        });

        Position = ImGui.GetMainViewport().Size / 2.0f - new Vector2(500.0f, 400.0f) / 2.0f;
        PositionCondition = ImGuiCond.Appearing;

        IsOpen = true;
    }

    public override void PreDraw() 
        => StyleModelV1.DalamudStandard.Push();

    protected override void DrawContents() {
        DrawHeader();

        using (var child = ImRaii.Child("RuleConfigurationTabChild", ImGui.GetContentRegionAvail() - FooterSize - ImGui.GetStyle().FramePadding)) {
            if (child) {
                view.Draw();
            }
        }
        
        DrawFooter();

        DrawPopup();
    }
    
    private void DrawPopup() {
        using var popup = ImRaii.Popup("Advanced Options");
        if (!popup) return;
                
        if (ImGui.Checkbox("Use Inclusive Logic", ref rule.InclusiveAnd)) {
            SortaKindaController.SortController.SaveConfig();
        }
    }

    public override void PostDraw() 
        => StyleModelV1.DalamudStandard.Pop();

    private void DrawHeader() {
        DrawColorEdit();
        ImGui.SameLine();
        DrawNameEdit();
        ImGui.SameLine();
        DrawDeleteButton();
        ImGuiHelpers.ScaledDummy(5.0f);
    }

    private void DrawColorEdit() {
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = ImGui.GetContentRegionMax().X / 4.0f - ImGuiHelpers.GlobalScale * 70.0f + ImGui.GetStyle().ItemSpacing.X / 2.0f });
        var imGuiColor = rule.Color;
        if (ImGui.ColorEdit4("##ColorConfig", ref imGuiColor, ImGuiColorEditFlags.NoInputs)) {
            rule.Color = imGuiColor;
        }
    }

    private void DrawNameEdit() {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionMax().X / 2.0f);
        var imGuiName = rule.Name;
        if (ImGui.InputText("##NameEdit", ref imGuiName, 1024, ImGuiInputTextFlags.AutoSelectAll)) {
            rule.Name = imGuiName;
            WindowName = $"SortaKinda Rule Configuration - {rule.Name}###RuleConfig{rule.Id}";
        }
    }

    private void DrawDeleteButton() {
        using var disabled = ImRaii.Disabled(!(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl));
        
        if (ImGui.Button("Delete", ImGuiHelpers.ScaledVector2(100.0f, 23.0f))) {
            ruleList.Remove(rule);
            IsOpen = false;
        }
        
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) {
            ImGui.SetTooltip("Hold Shift + Control while clicking to delete this rule");
        }
    }

    private void DrawFooter() {
        var buttonSize = ImGuiHelpers.ScaledVector2(100.0f, 23.0f);

        using var child = ImRaii.Child("##RuleConfigurationTabFooter", FooterSize - ImGui.GetStyle().FramePadding);
        if (!child) return;

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
    
    public override void OnClose() {
        SortaKindaController.SortController.SaveConfig();
        SortaKindaController.WindowManager.RemoveWindow(this);
    }
}