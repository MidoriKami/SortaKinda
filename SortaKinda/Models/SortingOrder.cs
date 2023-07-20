using System;
using Dalamud.Interface;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Abstracts;
using SortaKinda.Models.Enum;

namespace SortaKinda.Models;

public class SortingOrder
{
    public SortOrderDirection Direction = SortOrderDirection.Ascending;
    public SortOrderMode Mode = SortOrderMode.Alphabetically;
    public FillMode FillMode = FillMode.FillFromTop;

    public bool Compare(InventorySlot a, InventorySlot b)
    {
        var firstItem = a.LuminaData;
        var secondItem = b.LuminaData;

        switch (a.HasItem, b.HasItem)
        {
            // If both items are null, don't swap
            case (false, false): return false;

            // first slot empty, second slot full, if Ascending we want to left justify, move the items left, if Descending right justify, leave the empty slot on the left.
            case (false, true): return FillMode is FillMode.FillFromTop;

            // first slot full, second slot empty, if Ascending we want to left justify, and we have that already, if Descending right justify, move the item right
            case (true, false): return FillMode is FillMode.FillFromBottom;
            
            case (true, true) when firstItem is not null && secondItem is not null:
                var shouldSwap = ShouldSwap(firstItem, secondItem, ItemsMatch(firstItem, secondItem) ? SortOrderMode.Alphabetically : Mode);

                if (Direction is SortOrderDirection.Descending)
                    shouldSwap = !shouldSwap;

                return shouldSwap;
            
            // Something went horribly wrong... best not touch it and walk away.
            default: return false;
        }
    }

    private bool ItemsMatch(Item firstItem, Item secondItem) => Mode switch
    {
        SortOrderMode.ItemId => firstItem.RowId == secondItem.RowId,
        SortOrderMode.ItemLevel => firstItem.LevelItem.Row == secondItem.LevelItem.Row,
        SortOrderMode.Alphabetically => string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) == 0,
        SortOrderMode.SellPrice => firstItem.PriceLow == secondItem.PriceLow,
        _ => false,
    };
    
    private static bool ShouldSwap(Item firstItem, Item secondItem, SortOrderMode sortMode) => sortMode switch
    {
        SortOrderMode.ItemId => firstItem.RowId > secondItem.RowId,
        SortOrderMode.ItemLevel => firstItem.LevelItem.Row > secondItem.LevelItem.Row,
        SortOrderMode.Alphabetically => string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) > 0,
        SortOrderMode.SellPrice => firstItem.PriceLow > secondItem.PriceLow,
        _ => false,
    };

    public void Draw()
    {
        ImGui.Text("Ordering");
        ImGui.Separator();

        ImGui.PushItemWidth(200.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginCombo("Ordering Mode##OrderingCombo", Mode.ToString()))
        {
            foreach (var mode in global::System.Enum.GetValues<SortOrderMode>())
            {
                if (ImGui.Selectable(mode.ToString(), mode == Mode))
                {
                    Mode = mode;
                }
            }
            
            ImGui.EndCombo();
        }
        
        ImGui.PushItemWidth(200.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginCombo("Ordering Direction##OrderingDirectionCombo", Direction.ToString()))
        {
            foreach (var direction in global::System.Enum.GetValues<SortOrderDirection>())
            {
                if (ImGui.Selectable(direction.ToString(), direction == Direction))
                {
                    Direction = direction;
                }
            }
            
            ImGui.EndCombo();
        }
        
        ImGui.PushItemWidth(200.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginCombo("Fill Mode##OrderingDirectionCombo", FillMode.ToString()))
        {
            foreach (var fillMode in global::System.Enum.GetValues<FillMode>())
            {
                if (ImGui.Selectable(fillMode.ToString(), fillMode == FillMode))
                {
                    FillMode = fillMode;
                }
            }
            
            ImGui.EndCombo();
        }
    }
}