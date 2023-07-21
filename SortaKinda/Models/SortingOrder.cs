using System;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.Models.Enum;

namespace SortaKinda.Models;

public class SortingOrder : ISortingOrder
{
    public SortOrderDirection Direction = SortOrderDirection.Ascending;
    private FillMode fillMode = FillMode.Top;
    public SortOrderMode Mode = SortOrderMode.Alphabetically;

    public int Compare(IInventorySlot? x, IInventorySlot? y)
    {
        if (x is null) return 0;
        if (y is null) return 0;
        if (x.LuminaData is null) return 0;
        if (y.LuminaData is null) return 0;
        if (IsItemMatch(x.LuminaData, y.LuminaData)) return 0;
        if (CompareSlots(x, y)) return 1;
        return -1;
    }

    public bool CompareSlots(IInventorySlot a, IInventorySlot b)
    {
        var firstItem = a.LuminaData;
        var secondItem = b.LuminaData;

        switch (a.HasItem, b.HasItem)
        {
            // If both items are null, don't swap
            case (false, false): return false;

            // first slot empty, second slot full, if Ascending we want to left justify, move the items left, if Descending right justify, leave the empty slot on the left.
            case (false, true): return fillMode is FillMode.Top;

            // first slot full, second slot empty, if Ascending we want to left justify, and we have that already, if Descending right justify, move the item right
            case (true, false): return fillMode is FillMode.Bottom;

            case (true, true) when firstItem is not null && secondItem is not null:
                var shouldSwap = ShouldSwap(firstItem, secondItem, IsItemMatch(firstItem, secondItem) ? SortOrderMode.Alphabetically : Mode);

                if (Direction is SortOrderDirection.Descending)
                    shouldSwap = !shouldSwap;

                return shouldSwap;

            // Something went horribly wrong... best not touch it and walk away.
            default: return false;
        }
    }

    public void DrawConfigTabs()
    {
        if (ImGui.BeginTabItem("Ordering##OrderConfig"))
        {
            if (ImGui.BeginTable("##OrderTable", 2, ImGuiTableFlags.SizingStretchSame))
            {
                ImGui.TableNextColumn();
                ImGuiHelpers.ScaledDummy(1.0f);
                ImGui.Text("Order items using");
                ImGuiComponents.HelpMarker("The primary property of an item to use for ordering");
                DrawRadioEnum(ref Mode);

                ImGui.TableNextColumn();
                ImGuiHelpers.ScaledDummy(1.0f);
                ImGui.Text("Sort item by");
                ImGuiComponents.HelpMarker("Ascending: A -> Z\nDescending Z -> A");
                DrawRadioEnum(ref Direction);

                ImGuiHelpers.ScaledDummy(10.0f);
                ImGui.Text("Fill inventory slots from");
                ImGuiComponents.HelpMarker("Top - Items are shifted to the top left-most slots\nBottom - Items are shifted to the bottom right-most slots");
                DrawRadioEnum(ref fillMode);

                ImGui.EndTable();
            }

            ImGui.EndTabItem();
        }
    }

    public string GetSortingModeString()
    {
        return Mode.GetLabel();
    }

    private bool IsItemMatch(Item firstItem, Item secondItem)
    {
        return Mode switch
        {
            SortOrderMode.ItemId => firstItem.RowId == secondItem.RowId,
            SortOrderMode.ItemLevel => firstItem.LevelItem.Row == secondItem.LevelItem.Row,
            SortOrderMode.Alphabetically => string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) == 0,
            SortOrderMode.SellPrice => firstItem.PriceLow == secondItem.PriceLow,
            SortOrderMode.Rarity => firstItem.Rarity == secondItem.Rarity,
            _ => false
        };
    }

    private static bool ShouldSwap(Item firstItem, Item secondItem, SortOrderMode sortMode)
    {
        return sortMode switch
        {
            SortOrderMode.ItemId => firstItem.RowId > secondItem.RowId,
            SortOrderMode.ItemLevel => firstItem.LevelItem.Row > secondItem.LevelItem.Row,
            SortOrderMode.Alphabetically => string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) > 0,
            SortOrderMode.SellPrice => firstItem.PriceLow > secondItem.PriceLow,
            SortOrderMode.Rarity => firstItem.Rarity > secondItem.Rarity,
            _ => false
        };
    }

    private static void DrawRadioEnum<T>(ref T configValue) where T : global::System.Enum
    {
        foreach (global::System.Enum orderingMode in global::System.Enum.GetValues(configValue.GetType()))
        {
            var isSelected = Convert.ToInt32(configValue);
            if (ImGui.RadioButton(orderingMode.GetLabel(), ref isSelected, Convert.ToInt32(orderingMode)))
            {
                configValue = (T) orderingMode;
            }
        }
    }
}