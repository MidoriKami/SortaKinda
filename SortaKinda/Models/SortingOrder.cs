using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface;
using ImGuiNET;
using SortaKinda.Abstracts;
using SortaKinda.Models.Enum;

namespace SortaKinda.Models;

public class SortingOrder
{
    public SortOrderDirection Direction = SortOrderDirection.Ascending;
    public SortOrderMode Mode = SortOrderMode.Alphabetically;

    public bool Compare(InventorySlot a, InventorySlot b)
    {
        var firstItem = a.LuminaData;
        var secondItem = b.LuminaData;

        // If both items are null, don't swap
        if (!a.HasItem && !b.HasItem) return false;

        // first slot empty, second slot full, if Ascending we want to left justify, move the items left, if Descending right justify, leave the empty slot on the left.
        if (!a.HasItem && b.HasItem) return Direction is SortOrderDirection.Ascending;

        // first slot full, second slot empty, if Ascending we want to left justify, and we have that already, if Descending right justify, move the item right
        if (a.HasItem && !b.HasItem) return Direction is SortOrderDirection.Descending;

        if (firstItem is not null && secondItem is not null)
        {
            var shouldSwap = Mode switch
            {
                SortOrderMode.ItemId => firstItem.RowId < secondItem.RowId,
        
                SortOrderMode.ItemLevel => firstItem.LevelItem.Row == secondItem.LevelItem.Row ? 
                    string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) > 0 :
                    firstItem.LevelItem.Row < secondItem.LevelItem.Row,
        
                SortOrderMode.Alphabetically => string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) > 0,
            
                _ => false,
            };

            if (Direction is SortOrderDirection.Descending)
                shouldSwap = !shouldSwap;

            return shouldSwap;
        }

        throw new Exception("Logic Error, how tf did you get here?");
    }
    
    public IOrderedEnumerable<InventorySlot> OrderItems(IEnumerable<InventorySlot> slots)
        => Direction is SortOrderDirection.Ascending ? ApplyModeAscending(slots) : ApplyModeDescending(slots);

    private IOrderedEnumerable<InventorySlot> ApplyModeAscending(IEnumerable<InventorySlot> collection) => Mode switch
    {
        SortOrderMode.ItemId => collection.OrderBy(item => item.LuminaData?.LevelItem.Row),
        
        SortOrderMode.ItemLevel => collection
            .OrderBy(item => item.LuminaData?.LevelItem.Row)
            .ThenBy(item => item.LuminaData?.Name.RawString),
        
        SortOrderMode.Alphabetically => collection.OrderBy(item => item.LuminaData?.Name.RawString),
        
        _ => throw new ArgumentOutOfRangeException(),
    };
    
    private IOrderedEnumerable<InventorySlot> ApplyModeDescending(IEnumerable<InventorySlot> collection) => Mode switch
    {
        SortOrderMode.ItemId => collection.OrderByDescending(item => item.LuminaData?.LevelItem.Row),
        
        SortOrderMode.ItemLevel => collection
            .OrderByDescending(item => item.LuminaData?.LevelItem.Row)
            .ThenByDescending(item => item.LuminaData?.Name.RawString),
        
        SortOrderMode.Alphabetically => collection.OrderByDescending(item => item.LuminaData?.Name.RawString),
        _ => throw new ArgumentOutOfRangeException(),
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
    }
}