using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using ImGuiNET;
using SortaKinda.Interfaces;
using SortaKinda.Models;
using SortaKinda.System;

namespace SortaKinda.Views.SortControllerViews;

public class SortingRuleListView
{
    private readonly ISortController controller;
    private readonly List<SortingRule> sortingRules;
    private int? deletionRuleId;

    public SortingRuleListView(ISortController sortController, List<SortingRule> rules)
    {
        controller = sortController;
        sortingRules = rules;
    }

    public void Draw()
    {
        var region = ImGui.GetContentRegionAvail();
        var negativeOffset = new Vector2(0.0f, 23.0f * ImGuiHelpers.GlobalScale + ImGui.GetStyle().ItemSpacing.Y + 1.0f);
        deletionRuleId = null;
        
        if (ImGui.BeginChild("##RuleListChild", region - negativeOffset))
        {
            foreach (var index in Enumerable.Range(0, sortingRules.Count))
            {
                var rule = sortingRules[index];
                if (rule.Index != index)
                {
                    rule.Index = index;
                    controller.SaveConfig();
                }

                DrawRule(rule, index);
            }
        }
        ImGui.EndChild();

        AddNewRuleButton();

        if (deletionRuleId is { } ruleToDelete)
        {
            if (ruleToDelete == sortingRules.Count)
            {
                SortaKindaController.SortController.SelectedRuleIndex = sortingRules.Count - 1;
            }
            sortingRules.RemoveAt(ruleToDelete);
            controller.SaveConfig();
        }
    }

    private void AddNewRuleButton()
    {
        var region = ImGui.GetContentRegionAvail();

        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##AddNewRuleButton", region with { Y = 23.0f * ImGuiHelpers.GlobalScale }))
        {
            var newRule = new SortingRule
            {
                Color = GetRandomColor(),
                Id = Guid.NewGuid().ToString("N"),
                Name = "New Rule",
                Index = sortingRules.Count
            };

            sortingRules.Add(newRule);
            controller.SaveConfig();

            RuleConfigWindowController.AddRuleConfigWindow(newRule, sortingRules);
        }

        ImGui.PopFont();
    }

    private void DrawRule(SortingRule rule, int index)
    {
        DrawArrows(index);
        DrawRadioButton(index);
        DrawRuleEntry(rule, index);
    }

    private void DrawArrows(int index)
    {
        var rule = sortingRules[index];

        ImGui.BeginDisabled(index is 0 || index == sortingRules.Count - 1);
        if (ImGuiComponents.IconButton($"##DownButton{rule.Id}", FontAwesomeIcon.ArrowDown))
        {
            if (sortingRules.Count > 1)
            {
                sortingRules.Remove(rule);
                sortingRules.Insert(index + 1, rule);
                controller.SaveConfig();
            }
        }
        ImGui.EndDisabled();

        ImGui.SameLine();

        ImGui.BeginDisabled(index is 1 or 0);
        if (ImGuiComponents.IconButton($"##UpButton{rule.Id}", FontAwesomeIcon.ArrowUp))
        {
            if (sortingRules.Count > 1)
            {
                sortingRules.Remove(rule);
                sortingRules.Insert(index - 1, rule);
                controller.SaveConfig();
            }
        }
        ImGui.EndDisabled();
    }

    private static void DrawRadioButton(int index)
    {
        ImGui.SameLine();
        ImGui.RadioButton($"##Selected{index}", ref SortaKindaController.SortController.SelectedRuleIndex, index);
    }

    private void DrawRuleEntry(SortingRule rule, int index)
    {
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

    private void DrawDeleteButton(int index)
    {
        var hotkeyHeld = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl;
        var buttonSize = ImGuiHelpers.ScaledVector2(23.0f, 23.0f);

        if (!hotkeyHeld) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);

        ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonSize.X * 2.0f - ImGui.GetStyle().ItemSpacing.X);

        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Trash.ToIconString()}##{index}", buttonSize) && hotkeyHeld)
        {
            deletionRuleId = index;
        }
        ImGui.PopFont();

        if (!hotkeyHeld) ImGui.PopStyleVar();

        if (ImGui.IsItemHovered() && !hotkeyHeld)
        {
            ImGui.SetTooltip("Hold Shift + Control while clicking to delete this rule");
        }
    }

    private void DrawConfigButton(SortingRule rule)
    {
        var buttonSize = ImGuiHelpers.ScaledVector2(23.0f, 23.0f);
        ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonSize.X);

        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Cog.ToIconString()}##{rule.Id}", buttonSize))
        {
            RuleConfigWindowController.AddRuleConfigWindow(rule, sortingRules);
        }
        ImGui.PopFont();
    }

    private static Vector4 GetRandomColor()
    {
        var random = new Random();

        return new Vector4(
            random.Next(0, 255) / 255.0f,
            random.Next(0, 255) / 255.0f,
            random.Next(0, 255) / 255.0f,
            1.0f);
    }
}