using System;
using System.Drawing;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.Extensions;
using KamiLib.Window;
using KamiLib.Window.SelectionWindows;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Classes;
using SortaKinda.Controllers;

namespace SortaKinda.ViewComponents;

public class SortingRuleView(SortingRule rule) {
    private readonly TabBar tabBar = new("SortingRuleTabBar", [
        new ItemTypeFilterTab.ItemNameFilterTab(rule),
        new ItemTypeFilterTab(rule),
        new OtherFiltersTab(rule),
        new ToggleFiltersTab(rule),
        new SortOrderTab(rule)
    ], false);

    public void Draw() => tabBar.Draw();
}

public class ItemTypeFilterTab(SortingRule rule) : IOneColumnRuleConfigurationTab {
    public string Name => "Item Type Filter";
    
    public string FirstLabel => "Allowed Item Types";

    public bool Disabled => false;

    public SortingRule SortingRule { get; } = rule;

    public void DrawContents() {
        DrawSelectedTypes();
        
        if (ImGuiTweaks.IconButtonWithSize(Service.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.Plus, "openItemTypeSelect", ImGui.GetContentRegionAvail())) {
            System.WindowManager.AddWindow(new ItemUiCategorySelectionWindow(Service.PluginInterface) {
                MultiSelectionCallback = selections => {
                    foreach (var selected in selections) {
                        SortingRule.AllowedItemTypes.Add(selected.RowId);
                    }
                }
            }, WindowFlags.OpenImmediately);
        }
    }

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
        if (ImGuiTweaks.IconButtonWithSize(Service.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.Plus, "AddNameButton", buttonSize, "Add Name")) {
            if (newRegex.Regex is not null) {
                SortingRule.AllowedNameRegexes.Add(newRegex);
                newRegex = new UserRegex();
            }
        }
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
            if (Service.TextureProvider.GetFromGameIcon((uint) iconCategory) is not { } iconTexture) continue;

            if (ImGuiComponents.IconButton($"##RemoveButton{category}", FontAwesomeIcon.Trash)) {
                removalEntry = category;
            }

            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1.0f * ImGuiHelpers.GlobalScale);
            ImGui.Image(iconTexture.GetWrapOrEmpty().ImGuiHandle, ImGuiHelpers.ScaledVector2(20.0f, 20.0f));

            ImGui.SameLine();
            ImGui.TextUnformatted(entryName);
        }
        
        if (removalEntry is { } toRemove) {
            SortingRule.AllowedItemTypes.Remove(toRemove);
        }
    }
}

public class OtherFiltersTab(SortingRule rule) : ITwoColumnRuleConfigurationTab {
    public string Name => "Other Filters";
    
    public bool Disabled => false;
    
    public SortingRule SortingRule { get; } = rule;
    
    public string FirstLabel => "Range Filters";
    
    public string SecondLabel => "Item Rarity Filter";

    public void DrawLeftSideContents() {
        SortingRule.ItemLevelFilter.DrawConfig();
        SortingRule.VendorPriceFilter.DrawConfig();
    }

    public void DrawRightSideContents() {
        foreach (var enumValue in Enum.GetValues<ItemRarity>()) {
            var enabled = SortingRule.AllowedItemRarities.Contains(enumValue);
            if (ImGuiComponents.ToggleButton($"{enumValue.GetDescription()}", ref enabled)) {
                if (enabled) SortingRule.AllowedItemRarities.Add(enumValue);
                if (!enabled) SortingRule.AllowedItemRarities.Remove(enumValue);
            }

            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3.0f);
            ImGui.TextUnformatted(enumValue.GetDescription());
        }
    }
}

public class ToggleFiltersTab(SortingRule rule) : IOneColumnRuleConfigurationTab {
    public string Name => "Property Filters";
    
    public bool Disabled => false;
    
    public SortingRule SortingRule { get; } = rule;
    
    public string FirstLabel => "Property Filters";
    
    public void DrawContents() {
        SortingRule.UntradableFilter.DrawConfig();
        SortingRule.UniqueFilter.DrawConfig();
        SortingRule.DyeableFilter.DrawConfig();
        SortingRule.CollectableFilter.DrawConfig();
        SortingRule.RepairableFilter.DrawConfig();
    }
}

public class SortOrderTab(SortingRule rule) : ITwoColumnRuleConfigurationTab {
    public string Name => "Sort Order";
    
    public bool Disabled => false;
    
    public SortingRule SortingRule { get; } = rule;
    
    public string FirstLabel => "Sort By";
    
    public string SecondLabel => "Sort Options";

    public void DrawLeftSideContents() {
        ImGui.Text("Order items using");
        ImGuiComponents.HelpMarker("The primary property of an item to use for ordering");
        var sortMode = SortingRule.SortMode;
        DrawRadioEnum(ref sortMode);

        SortingRule.SortMode = sortMode;
    }

    public void DrawRightSideContents() {
        ImGui.Text("Sort item by");
        ImGuiComponents.HelpMarker("Ascending: A -> Z\nDescending Z -> A");
        var sortDirection = SortingRule.Direction;
        DrawRadioEnum(ref sortDirection);

        ImGuiHelpers.ScaledDummy(8.0f);
        ImGui.Text("Fill inventory slots from");
        ImGuiComponents.HelpMarker("Top - Items are shifted to the top left-most slots\nBottom - Items are shifted to the bottom right-most slots");
        var fillMode = SortingRule.FillMode;
        DrawRadioEnum(ref fillMode);

        SortingRule.Direction = sortDirection;
        SortingRule.FillMode = fillMode;
    }

    private static void DrawRadioEnum<T>(ref T configValue) where T : Enum {
        foreach (Enum value in Enum.GetValues(configValue.GetType())) {
            var isSelected = Convert.ToInt32(configValue);
            if (ImGui.RadioButton($"{value.GetDescription()}##{configValue.GetType()}", ref isSelected, Convert.ToInt32(value))) {
                configValue = (T) value;
            }
        }
    }
}