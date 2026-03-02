using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using KamiLib.Classes;
using KamiLib.Extensions;
using KamiLib.Window;
using KamiLib.Window.SelectionWindows;
using Lumina.Excel.Sheets;
using SortaKinda.Classes;

namespace SortaKinda.ViewComponents;

public class SortingRuleView(SortingRule rule) {
    private readonly TabBar tabBar = new("SortingRuleTabBar", [
        new ItemTypeFilterTab.ItemNameFilterTab(rule),
        new ItemTypeFilterTab(rule),
        new OtherFiltersTab(rule),
        new ToggleFiltersTab(rule),
        new SortOrderTab(rule),
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
                },
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
            if (Service.DataManager.GetExcelSheet<ItemUICategory>().GetRow(category) is not { RowId: not 0, Icon: var iconCategory, Name: var entryName }) continue;
            if (Service.TextureProvider.GetFromGameIcon((uint) iconCategory) is not { } iconTexture) continue;

            if (ImGuiComponents.IconButton($"##RemoveButton{category}", FontAwesomeIcon.Trash)) {
                removalEntry = category;
            }

            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1.0f * ImGuiHelpers.GlobalScale);
            ImGui.Image(iconTexture.GetWrapOrEmpty().Handle, ImGuiHelpers.ScaledVector2(20.0f, 20.0f));

            ImGui.SameLine();
            ImGui.TextUnformatted(entryName.ExtractText());
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
        SortingRule.LevelFilter.DrawConfig();
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

public class SortOrderTab(SortingRule rule) : IRuleConfigurationTab {
    // UI-local selection used by the "Add Mode" button.
    // The value is only persisted once added to AdditionalSortRules.
    private SortOrderMode additionalSortMode = SortOrderMode.ItemId;
    private SortOrderDirection additionalSortDirection = SortOrderDirection.Ascending;

    public string Name => "Sort Order";
    
    public bool Disabled => false;
    
    public SortingRule SortingRule { get; } = rule;

    public void DrawConfigurationTab() {
        EnsureWindowHeightForSortOrder();

        // Read old config values once before drawing, so old rule exports continue to render correctly.
        SortingRule.MigrateLegacyAdditionalSortModes();

        var removeIndex = -1;
        var moveUpIndex = -1;
        var moveDownIndex = -1;

        using var table = ImRaii.Table("##SortOrderPrimaryTable", 2, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.BordersInnerV, ImGui.GetContentRegionAvail());
        if (!table) return;

        ImGui.TableNextColumn();
        ImGui.TextUnformatted("Sort By");
        ImGui.Separator();

        ImGui.TableNextColumn();
        ImGui.TextUnformatted("Sort Options");
        ImGui.Separator();

        ImGui.TableNextColumn();
        DrawLeftSideContents();

        ImGui.TableNextColumn();
        DrawRightSideContents();

        // Tie-breaker section.
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.Text("Then by");
        ImGuiComponents.HelpMarker("Additional sort modes are applied in order when earlier modes tie.");

        ImGui.TableNextColumn();
        ImGuiHelpers.ScaledDummy(1.0f);

        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.Separator();
        ImGui.TableNextColumn();
        ImGui.Separator();

        if (SortingRule.AdditionalSortRules.Count is 0) {
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.TextColored(KnownColor.Orange.Vector(), "No additional modes");
            ImGui.TableNextColumn();
            ImGuiHelpers.ScaledDummy(1.0f);
        }

        for (var index = 0; index < SortingRule.AdditionalSortRules.Count; index++) {
            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            DrawAdditionalSortModeLeft(index, ref moveUpIndex, ref moveDownIndex, ref removeIndex);

            ImGui.TableNextColumn();
            DrawAdditionalSortModeRight(index);
        }

        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.Separator();
        ImGui.TableNextColumn();
        ImGui.Separator();

        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        DrawAdditionalSortAddLeft();
        ImGui.TableNextColumn();
        DrawAdditionalSortAddRight();

        // Apply queued list mutations after rendering to avoid mutating during iteration.
        if (moveUpIndex > 0) {
            (SortingRule.AdditionalSortRules[moveUpIndex - 1], SortingRule.AdditionalSortRules[moveUpIndex]) = (SortingRule.AdditionalSortRules[moveUpIndex], SortingRule.AdditionalSortRules[moveUpIndex - 1]);
        }

        if (moveDownIndex >= 0 && moveDownIndex < SortingRule.AdditionalSortRules.Count - 1) {
            (SortingRule.AdditionalSortRules[moveDownIndex], SortingRule.AdditionalSortRules[moveDownIndex + 1]) = (SortingRule.AdditionalSortRules[moveDownIndex + 1], SortingRule.AdditionalSortRules[moveDownIndex]);
        }

        if (removeIndex >= 0 && removeIndex < SortingRule.AdditionalSortRules.Count) {
            SortingRule.AdditionalSortRules.RemoveAt(removeIndex);
        }
    }

    private void DrawLeftSideContents() {
        ImGui.Text("Order items using");
        ImGuiComponents.HelpMarker("The primary property of an item to use for ordering");
        var sortMode = SortingRule.SortMode;
        DrawRadioEnum(ref sortMode);
        SortingRule.SortMode = sortMode;
    }

    private void DrawRightSideContents() {
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

    private void DrawAdditionalSortModeLeft(int index, ref int moveUpIndex, ref int moveDownIndex, ref int removeIndex) {
        var sortRule = SortingRule.AdditionalSortRules[index];
        using var _ = ImRaii.PushId(index);

        using (ImRaii.Disabled(index is 0)) {
            if (ImGuiComponents.IconButton($"##AdditionalUp{index}", FontAwesomeIcon.ArrowUp)) {
                moveUpIndex = index;
            }
        }

        ImGui.SameLine();
        using (ImRaii.Disabled(index == SortingRule.AdditionalSortRules.Count - 1)) {
            if (ImGuiComponents.IconButton($"##AdditionalDown{index}", FontAwesomeIcon.ArrowDown)) {
                moveDownIndex = index;
            }
        }

        ImGui.SameLine();
        if (ImGuiComponents.IconButton($"##AdditionalDelete{index}", FontAwesomeIcon.Trash)) {
            removeIndex = index;
        }

        ImGui.SameLine();
        ImGui.TextUnformatted(sortRule.Mode.GetDescription());
    }

    private void DrawAdditionalSortModeRight(int index) {
        // Direction is stored per tie-breaker rule.
        var direction = SortingRule.AdditionalSortRules[index].Direction;
        if (ImGui.RadioButton($"Asc##AdditionalAsc{index}", direction is SortOrderDirection.Ascending)) {
            SortingRule.AdditionalSortRules[index].Direction = SortOrderDirection.Ascending;
        }

        ImGui.SameLine();
        if (ImGui.RadioButton($"Desc##AdditionalDesc{index}", direction is SortOrderDirection.Descending)) {
            SortingRule.AdditionalSortRules[index].Direction = SortOrderDirection.Descending;
        }
    }

    private void DrawAdditionalSortAddLeft() {
        ImGui.SetNextItemWidth(-1.0f);
        if (ImGui.BeginCombo("##AdditionalSortModeCombo", additionalSortMode.GetDescription())) {
            foreach (var value in Enum.GetValues<SortOrderMode>()) {
                if (ImGui.Selectable(value.GetDescription(), value == additionalSortMode)) {
                    additionalSortMode = value;
                }
            }
            ImGui.EndCombo();
        }
    }

    private void DrawAdditionalSortAddRight() {
        // Direction selected here becomes the default for the next added tie-breaker entry.
        var newDirection = additionalSortDirection;
        DrawSortDirectionRadioPair("##NewAdditionalDirection", ref newDirection, true);
        additionalSortDirection = newDirection;

        var buttonSize = ImGuiHelpers.ScaledVector2(85.0f, 0.0f);
        ImGui.SameLine(ImGui.GetContentRegionAvail().X - buttonSize.X);
        if (ImGui.Button("Add Mode", buttonSize)) {
            SortingRule.AdditionalSortRules.Add(new AdditionalSortRule {
                Mode = additionalSortMode,
                Direction = additionalSortDirection
            });
        }
    }

    private static void DrawSortDirectionRadioPair(string id, ref SortOrderDirection direction, bool shortLabels) {
        var ascLabel = shortLabels ? "Asc" : "Ascending";
        var descLabel = shortLabels ? "Desc" : "Descending";

        if (ImGui.RadioButton($"{ascLabel}##{id}Asc", direction is SortOrderDirection.Ascending)) {
            direction = SortOrderDirection.Ascending;
        }

        ImGui.SameLine();
        if (ImGui.RadioButton($"{descLabel}##{id}Desc", direction is SortOrderDirection.Descending)) {
            direction = SortOrderDirection.Descending;
        }
    }

    private static void EnsureWindowHeightForSortOrder() {
        var minimumHeight = ImGuiHelpers.ScaledVector2(0.0f, 610.0f).Y;
        var size = ImGui.GetWindowSize();
        if (size.Y < minimumHeight) {
            ImGui.SetWindowSize(new Vector2(size.X, minimumHeight));
        }
    }
}