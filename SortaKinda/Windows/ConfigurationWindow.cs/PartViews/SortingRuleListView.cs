using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using ImGuiNET;
using SortaKinda.Models;
using SortaKinda.System;
using SortaKinda.Views.Windows;

namespace SortaKinda.Views.SortControllerViews;

public class SortingRuleListView(SortController sortController, List<SortingRule> rules) {
    private int? deletionRuleId;

    public void Draw() {
        var region = ImGui.GetContentRegionAvail();
        var negativeOffset = new Vector2(0.0f, 23.0f * ImGuiHelpers.GlobalScale + ImGui.GetStyle().ItemSpacing.Y + 1.0f);
        deletionRuleId = null;
        
        if (ImGui.BeginChild("##RuleListChild", region - negativeOffset)) {
            foreach (var index in Enumerable.Range(0, rules.Count)) {
                var rule = rules[index];
                if (rule.Index != index) {
                    rule.Index = index;
                    sortController.SaveConfig();
                }

                DrawRule(rule, index);
            }
        }
        ImGui.EndChild();

        AddNewRuleButton();

        if (deletionRuleId is { } ruleToDelete) {
            if (ruleToDelete == rules.Count) {
                SortaKindaController.SortController.SelectedRuleIndex = rules.Count - 1;
            }
            rules.RemoveAt(ruleToDelete);
            sortController.SaveConfig();
        }
    }

    private void AddNewRuleButton() {
        var region = ImGui.GetContentRegionAvail();

        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##AddNewRuleButton", region with { Y = 23.0f * ImGuiHelpers.GlobalScale })) {
            var newRule = new SortingRule {
                Color = GetRandomColor(),
                Id = Guid.NewGuid().ToString("N"),
                Name = "New Rule",
                Index = rules.Count
            };

            rules.Add(newRule);
            sortController.SaveConfig();

            SortaKindaController.WindowManager.AddWindow(new RuleConfigWindow(newRule, rules));
        }

        ImGui.PopFont();
    }

    private void DrawRule(SortingRule rule, int index) {
        DrawArrows(index);
        DrawRadioButton(index);
        DrawRuleEntry(rule, index);
    }

    private void DrawArrows(int index) {
        var rule = rules[index];

        ImGui.BeginDisabled(index is 0 || index == rules.Count - 1);
        if (ImGuiComponents.IconButton($"##DownButton{rule.Id}", FontAwesomeIcon.ArrowDown)) {
            if (rules.Count > 1) {
                rules.Remove(rule);
                rules.Insert(index + 1, rule);
                sortController.SaveConfig();
            }
        }
        ImGui.EndDisabled();

        ImGui.SameLine();

        ImGui.BeginDisabled(index is 1 or 0);
        if (ImGuiComponents.IconButton($"##UpButton{rule.Id}", FontAwesomeIcon.ArrowUp)) {
            if (rules.Count > 1) {
                rules.Remove(rule);
                rules.Insert(index - 1, rule);
                sortController.SaveConfig();
            }
        }
        ImGui.EndDisabled();
    }

    private static void DrawRadioButton(int index) {
        ImGui.SameLine();
        ImGui.RadioButton($"##Selected{index}", ref SortaKindaController.SortController.SelectedRuleIndex, index);
    }

    private void DrawRuleEntry(SortingRule rule, int index) {
        ImGui.SameLine();

        var ruleColor = rule.Color;
        ImGui.ColorEdit4($"##{rule.Id}ColorEdit", ref ruleColor, ImGuiColorEditFlags.NoPicker | ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoTooltip);

        ImGui.SameLine();
        ImGui.TextUnformatted(rule.Name);

        ImGui.BeginDisabled(rule.Id is SortController.DefaultId);
        DrawDeleteButton(index);
        ImGui.EndDisabled();

        ImGui.BeginDisabled(rule.Id is SortController.DefaultId);
        DrawConfigButton(rule);
        ImGui.EndDisabled();
    }

    private void DrawDeleteButton(int index) {
        var hotkeyHeld = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl;
        var buttonSize = ImGuiHelpers.ScaledVector2(23.0f, 23.0f);

        if (!hotkeyHeld) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);

        ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonSize.X * 2.0f - ImGui.GetStyle().ItemSpacing.X);

        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Trash.ToIconString()}##{index}", buttonSize) && hotkeyHeld) {
            deletionRuleId = index;
        }
        ImGui.PopFont();

        if (!hotkeyHeld) ImGui.PopStyleVar();

        if (ImGui.IsItemHovered() && !hotkeyHeld) {
            ImGui.SetTooltip("Hold Shift + Control while clicking to delete this rule");
        }
    }

    private void DrawConfigButton(SortingRule rule) {
        var buttonSize = ImGuiHelpers.ScaledVector2(23.0f, 23.0f);
        ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonSize.X);

        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Cog.ToIconString()}##{rule.Id}", buttonSize)) {
            SortaKindaController.WindowManager.AddWindow(new RuleConfigWindow(rule, rules));
        }
        ImGui.PopFont();
    }

    private static Vector4 GetRandomColor() {
        var random = new Random();

        return new Vector4(
            random.Next(0, 255) / 255.0f,
            random.Next(0, 255) / 255.0f,
            random.Next(0, 255) / 255.0f,
            1.0f);
    }
}