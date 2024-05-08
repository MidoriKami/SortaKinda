using System.Drawing;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.TabBar;
using SortaKinda.Configuration;
using SortaKinda.Models;
using SortaKinda.Windows.SortingRuleWindow.Components;

namespace SortaKinda.Windows.SortingRuleWindow;

public class ItemNameFilterTab(SortingRule rule) : IOneColumnRuleConfigurationTab {
    private UserRegex newRegex = new();
    private bool setNameFocus = true;

    public string Name => "Item Name Filter";
    
    public bool Disabled => false;
    
    public string FirstLabel => "Allowed Item Names";
    
    public SortingRule SortingRule { get; } = rule;

    public void DrawContents() {
        DrawFilteredNames();
        DrawAddItemNameInput();
    }

    private void DrawFilteredNames() {
        UserRegex? removalRegex = null;

        using var child = ImRaii.Child("##NameFilterChild", ImGuiHelpers.ScaledVector2(0.0f, -50.0f));
        if (!child) return;
        
        if (SortingRule.AllowedNameRegexes.Count is 0) {
            ImGui.TextColored(KnownColor.Orange.Vector(), "Nothing Filtered");
        }

        foreach (var userRegex in SortingRule.AllowedNameRegexes) {
            if (ImGuiComponents.IconButton($"##RemoveNameRegex{userRegex.Text}", FontAwesomeIcon.Trash)) {
                removalRegex = userRegex;
            }
            ImGui.SameLine();
            ImGui.TextUnformatted(userRegex.Text);
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
                newRegex = new UserRegex();
            }
            setNameFocus = true;
        }

        ImGui.SameLine();

        using var disabled = ImRaii.Disabled(newRegex.Regex is null || newRegex.Text.IsNullOrEmpty());
        if (ImGuiTweaks.IconButtonWithSize(FontAwesomeIcon.Plus, "AddNameButton", buttonSize, "Add Name")) {
            if (newRegex.Regex is not null) {
                SortingRule.AllowedNameRegexes.Add(newRegex);
                newRegex = new UserRegex();
            }
        }
    }
}