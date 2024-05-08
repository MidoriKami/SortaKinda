using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.TabBar;
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

        using (var ruleList = ImRaii.Child("RuleListChild", region - negativeOffset)) {
            if (ruleList) {
                foreach (var index in Enumerable.Range(0, rules.Count)) {
                    var rule = rules[index];
                    if (rule.Index != index) {
                        rule.Index = index;
                        sortController.SaveConfig();
                    }

                    DrawRule(rule, index);
                }
            }
        }
        
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
        if (ImGuiTweaks.IconButtonWithSize(FontAwesomeIcon.Plus, "AddNewRuleButton", ImGui.GetContentRegionAvail() with { Y = 23.0f * ImGuiHelpers.GlobalScale })) {
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
    }

    private void DrawRule(SortingRule rule, int index) {
        DrawArrows(index);
        DrawRadioButton(index);
        DrawRuleEntry(rule, index);
    }

    private void DrawArrows(int index) {
        var rule = rules[index];

        using (var _ = ImRaii.Disabled(index is 0 || index == rules.Count - 1)) {
            if (ImGuiComponents.IconButton($"##DownButton{rule.Id}", FontAwesomeIcon.ArrowDown)) {
                if (rules.Count > 1) {
                    rules.Remove(rule);
                    rules.Insert(index + 1, rule);
                    sortController.SaveConfig();
                }
            }
        }

        ImGui.SameLine();
        
        using (var _ = ImRaii.Disabled(index is 1 or 0)) {
            if (ImGuiComponents.IconButton($"##UpButton{rule.Id}", FontAwesomeIcon.ArrowUp)) {
                if (rules.Count > 1) {
                    rules.Remove(rule);
                    rules.Insert(index - 1, rule);
                    sortController.SaveConfig();
                }
            }
        }
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
        var buttonSize = ImGuiHelpers.ScaledVector2(23.0f, 23.0f);

        ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonSize.X * 2.0f - ImGui.GetStyle().ItemSpacing.X);

        using (var _ = ImRaii.Disabled(!(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl))) {
            if (ImGuiTweaks.IconButtonWithSize(FontAwesomeIcon.Trash, index.ToString(), buttonSize)) {
                deletionRuleId = index;
            }
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) {
            ImGui.SetTooltip("Hold Shift + Control while clicking to delete this rule");
        }
    }

    private void DrawConfigButton(SortingRule rule) {
        var buttonSize = ImGuiHelpers.ScaledVector2(23.0f, 23.0f);
        ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonSize.X);

        if (ImGuiTweaks.IconButtonWithSize(FontAwesomeIcon.Cog, rule.Id, buttonSize)) {
            SortaKindaController.WindowManager.AddWindow(new RuleConfigWindow(rule, rules));
        }
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