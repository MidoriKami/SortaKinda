using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Models.Enum;
using SortaKinda.System;

namespace SortaKinda.Models;

public unsafe class SortingOrder
{
    public SortOrderDirection Direction = SortOrderDirection.Ascending;
    public SortOrderMode Mode = SortOrderMode.Alphabetically;

    public IOrderedEnumerable<Pointer<ItemOrderModuleSorterItemEntry>> OrderItems(IEnumerable<Pointer<ItemOrderModuleSorterItemEntry>> items, InventoryType inventoryType) 
        => Direction is SortOrderDirection.Ascending ? ApplyModeAscending(items, inventoryType) : ApplyModeDescending(items, inventoryType);

    private IOrderedEnumerable<Pointer<ItemOrderModuleSorterItemEntry>> ApplyModeAscending(IEnumerable<Pointer<ItemOrderModuleSorterItemEntry>> collection, InventoryType inventoryType) => Mode switch
    {
        SortOrderMode.ItemId => collection.OrderBy(item => GetLuminaData(item, inventoryType)?.LevelItem.Row),
        
        SortOrderMode.ItemLevel => collection
            .OrderBy(item => GetLuminaData(item, inventoryType)?.LevelItem.Row)
            .ThenBy(item => GetLuminaData(item, inventoryType)?.Name.RawString),
        
        SortOrderMode.Alphabetically => collection.OrderBy(item => GetLuminaData(item, inventoryType)?.Name.RawString),
        
        _ => throw new ArgumentOutOfRangeException(),
    };
    
    private IOrderedEnumerable<Pointer<ItemOrderModuleSorterItemEntry>> ApplyModeDescending(IEnumerable<Pointer<ItemOrderModuleSorterItemEntry>> collection, InventoryType inventoryType) => Mode switch
    {
        SortOrderMode.ItemId => collection.OrderByDescending(item => GetLuminaData(item, inventoryType)?.LevelItem.Row ),
        
        SortOrderMode.ItemLevel => collection
            .OrderByDescending(item => GetLuminaData(item, inventoryType)?.LevelItem.Row)           
            .ThenByDescending(item => GetLuminaData(item, inventoryType)?.Name.RawString),
        
        SortOrderMode.Alphabetically => collection.OrderByDescending(item => GetLuminaData(item, inventoryType)?.Name.RawString),
        _ => throw new ArgumentOutOfRangeException(),
    };

    private Item? GetLuminaData(Pointer<ItemOrderModuleSorterItemEntry> entry, InventoryType type) 
        => LuminaCache<Item>.Instance.GetRow(InventoryController.GetItemForSlot(type, entry)->ItemID);
    
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