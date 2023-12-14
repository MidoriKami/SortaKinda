using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using ImGuiNET;
using SortaKinda.Interfaces;
using SortaKinda.Models;

namespace SortaKinda.Views.Tabs;

public class ItemNameFilterTab : IOneColumnRuleConfigurationTab
{
    private UserRegex newRegex = new();
    private bool setNameFocus = true;

    public ItemNameFilterTab(ISortingRule rule) {
        SortingRule = rule;
    }

    public string TabName => "Item Name Filter";
    public bool Enabled => true;
    public string FirstLabel => "Allowed Item Names";
    public ISortingRule SortingRule { get; }

    public void DrawContents() {
        DrawFilteredNames();
        DrawAddItemNameInput();
    }

    private void DrawFilteredNames() {
        string? removalString = null;
        UserRegex? removalRegex = null;

        if (ImGui.BeginChild("##NameFilterChild", new Vector2(0.0f, -50.0f))) {
            if (SortingRule.AllowedItemNames.Count is 0 && SortingRule.AllowedNameRegexes.Count is 0) {
                ImGui.TextColored(KnownColor.Orange.Vector(), "Nothing Filtered");
            }

            foreach (var name in SortingRule.AllowedItemNames) {
                if (ImGuiComponents.IconButton($"##RemoveName{name}", FontAwesomeIcon.Trash)) {
                    removalString = name;
                }
                ImGui.SameLine();
                ImGui.TextUnformatted(name);
            }

            foreach (var userRegex in SortingRule.AllowedNameRegexes) {
                if (ImGuiComponents.IconButton($"##RemoveNameRegex{userRegex.Text}", FontAwesomeIcon.Trash)) {
                    removalRegex = userRegex;
                }
                ImGui.SameLine();
                ImGui.TextUnformatted(userRegex.Text);
            }
        }
        ImGui.EndChild();

        if (removalString is { } toRemove) {
            SortingRule.AllowedItemNames.Remove(toRemove);
        }

        if (removalRegex is { } toRemoveRegex) {
            SortingRule.AllowedNameRegexes.Remove(toRemoveRegex);
        }
    }

    private void DrawAddItemNameInput() {
        var buttonSize = ImGuiHelpers.ScaledVector2(25.0f, 23.0f);

        if (setNameFocus || ImGui.IsWindowAppearing()) {
            ImGui.SetKeyboardFocusHere();
            setNameFocus = false;
        }

        ImGui.TextColored(KnownColor.Gray.Vector(), "Supports Regex for item name filtering");

        if (UserRegex.DrawRegexInput("##NewName", ref newRegex, "Item Name", null, ImGui.GetContentRegionAvail().X - buttonSize.X - ImGui.GetStyle().ItemSpacing.X, ImGui.GetColorU32(KnownColor.OrangeRed.Vector()))) {
            if (newRegex.Regex is not null) {
                SortingRule.AllowedNameRegexes.Add(newRegex);
            }
            setNameFocus = true;
        }

        ImGui.SameLine();

        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##AddNameButton", buttonSize)) {
            if (newRegex.Regex is not null) {
                SortingRule.AllowedNameRegexes.Add(newRegex);
            }
        }
        ImGui.PopFont();

        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip("Add Name");
        }
    }
}