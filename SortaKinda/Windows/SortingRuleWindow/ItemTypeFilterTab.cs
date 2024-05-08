using System.Drawing;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.TabBar;
using KamiLib.Window;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.Models;
using SortaKinda.System;

namespace SortaKinda.Views.Tabs;

public class ItemTypeFilterTab(SortingRule rule) : IOneColumnRuleConfigurationTab {
    public string Name => "Item Type Filter";
    
    public string FirstLabel => "Allowed Item Types";

    public bool Disabled => false;

    public SortingRule SortingRule { get; } = rule;

    public void DrawContents() {
        DrawSelectedTypes();
        
        if (ImGuiTweaks.IconButtonWithSize(FontAwesomeIcon.Plus, "openItemTypeSelect", ImGui.GetContentRegionAvail())) {
            SortaKindaController.WindowManager.AddWindow(new ItemUICategorySelectionWindow(Service.PluginInterface) {
                MultiSelectionCallback = selections => {
                    foreach (var selected in selections) {
                        SortingRule.AllowedItemTypes.Add(selected.RowId);
                    }
                }
            }, WindowFlags.OpenImmediately);
        }
    }

    private void DrawSelectedTypes() {
        uint? removalEntry = null;

        using var itemFilterChild = ImRaii.Child("##ItemFilterChild", ImGuiHelpers.ScaledVector2(0.0f, -30.0f));
        if (!itemFilterChild) return;
        
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
        
        if (removalEntry is { } toRemove) {
            SortingRule.AllowedItemTypes.Remove(toRemove);
        }
    }
}