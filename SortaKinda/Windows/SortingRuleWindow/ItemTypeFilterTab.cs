using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Window;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.Models;
using SortaKinda.System;

namespace SortaKinda.Views.Tabs;

public class ItemTypeFilterTab(SortingRule rule) : ITwoColumnRuleConfigurationTab {
    private List<ItemUICategory>? searchResults;
    private string searchString = string.Empty;

    public string Name => "Item Type Filter";
    
    public string FirstLabel => "Allowed Item Types";
    
    public string SecondLabel => "Item Type Search";
    
    public bool Disabled => false;

    public SortingRule SortingRule { get; } = rule;

    public void DrawLeftSideContents() {
        uint? removalEntry = null;
        if (ImGui.BeginChild("##ItemFilterChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding, false)) {
            if (SortingRule.AllowedItemTypes.Count is 0) {
                ImGui.TextColored(KnownColor.Orange.Vector(), "Nothing Filtered");
            }

            foreach (var category in SortingRule.AllowedItemTypes) {
                if (Service.DataManager.GetExcelSheet<ItemUICategory>()!.GetRow(category) is not { Icon: var iconCategory, Name.RawString: var entryName }) continue;
                if (Service.TextureProvider.GetIcon((uint) iconCategory) is not { } iconTexture) continue;

                if (ImGuiComponents.IconButton($"##RemoveButton{category}", FontAwesomeIcon.Trash)) {
                    removalEntry = category;
                }

                ImGui.SameLine();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1.0f * ImGuiHelpers.GlobalScale);
                ImGui.Image(iconTexture.ImGuiHandle, ImGuiHelpers.ScaledVector2(20.0f, 20.0f));

                ImGui.SameLine();
                ImGui.TextUnformatted(entryName);
            }
        }
        ImGui.EndChild();

        if (removalEntry is { } toRemove) {
            SortingRule.AllowedItemTypes.Remove(toRemove);
        }
    }

    public void DrawRightSideContents() {
        var buttonSize = ImGuiHelpers.ScaledVector2(23.0f, 23.0f);

        if (ImGui.IsWindowAppearing()) {
            ImGui.SetKeyboardFocusHere();
        }

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttonSize.X - ImGui.GetStyle().ItemSpacing.X);
        if (ImGui.InputTextWithHint("##SearchBox", "Search...", ref searchString, 1024, ImGuiInputTextFlags.AutoSelectAll)) {
            if (searchString.IsNullOrEmpty()) {
                searchResults = null;
            }
            else {
                searchResults = Service.DataManager.GetExcelSheet<ItemUICategory>()!
                    .Where(entry => entry.Name.RawString.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Search.ToIconString()}##ShowAll", buttonSize)) {
            SortaKindaController.WindowManager.AddWindow(new ItemUICategorySelectionWindow(Service.PluginInterface) {
                MultiSelectionCallback = results => {
                    foreach (var result in results) {
                        SortingRule.AllowedItemTypes.Add(result.RowId);
                    }
                }
            });
        }
        ImGui.PopFont();

        if (ImGui.BeginChild("##SearchResultsChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding, false)) {
            if (searchResults is null || searchResults.Count is 0) {
                ImGui.TextColored(KnownColor.Gray.Vector(), "No Results");
            }
            else {
                foreach (var result in searchResults.Where(result => result.Name.RawString is not "")) {
                    if (!SortingRule.AllowedItemTypes.Contains(result.RowId)) {
                        if (ImGuiComponents.IconButton($"##AddCategoryButton{result.RowId}", FontAwesomeIcon.Plus)) {
                            SortingRule.AllowedItemTypes.Add(result.RowId);
                        }
                    }
                    else {
                        if (ImGuiComponents.IconButton($"##RemoveCategoryButton{result.RowId}", FontAwesomeIcon.Trash)) {
                            SortingRule.AllowedItemTypes.Remove(result.RowId);
                        }
                    }

                    if (Service.TextureProvider.GetIcon((uint) result.Icon) is { } icon) {
                        ImGui.SameLine();
                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1.0f * ImGuiHelpers.GlobalScale);
                        ImGui.Image(icon.ImGuiHandle, ImGuiHelpers.ScaledVector2(20.0f, 20.0f));
                    }

                    ImGui.SameLine();
                    ImGui.TextUnformatted(result.Name.RawString);
                }
            }
        }
        ImGui.EndChild();
    }
}