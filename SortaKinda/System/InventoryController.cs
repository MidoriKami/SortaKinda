using System;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;

namespace SortaKinda.System;

public unsafe partial class InventoryController
{
    public static ItemOrderModuleSorter* GetInventorySorter(InventoryType type) => type switch
    {
        InventoryType.Inventory1 => UIModule.Instance()->GetItemOrderModule()->InventorySorter,
        InventoryType.Inventory2 => UIModule.Instance()->GetItemOrderModule()->InventorySorter,
        InventoryType.Inventory3 => UIModule.Instance()->GetItemOrderModule()->InventorySorter,
        InventoryType.Inventory4 => UIModule.Instance()->GetItemOrderModule()->InventorySorter,
        InventoryType.ArmoryMainHand => UIModule.Instance()->GetItemOrderModule()->ArmouryMainHandSorter,
        InventoryType.ArmoryOffHand => UIModule.Instance()->GetItemOrderModule()->ArmouryOffHandSorter,
        InventoryType.ArmoryHead => UIModule.Instance()->GetItemOrderModule()->ArmouryHeadSorter,
        InventoryType.ArmoryBody => UIModule.Instance()->GetItemOrderModule()->ArmouryBodySorter,
        InventoryType.ArmoryHands => UIModule.Instance()->GetItemOrderModule()->ArmouryHandsSorter,
        InventoryType.ArmoryLegs => UIModule.Instance()->GetItemOrderModule()->ArmouryLegsSorter,
        InventoryType.ArmoryFeets => UIModule.Instance()->GetItemOrderModule()->ArmouryFeetSorter,
        InventoryType.ArmoryEar => UIModule.Instance()->GetItemOrderModule()->ArmouryEarsSorter,
        InventoryType.ArmoryNeck => UIModule.Instance()->GetItemOrderModule()->ArmouryNeckSorter,
        InventoryType.ArmoryWrist => UIModule.Instance()->GetItemOrderModule()->ArmouryWristsSorter,
        InventoryType.ArmoryRings => UIModule.Instance()->GetItemOrderModule()->ArmouryRingsSorter,
        _ => throw new Exception($"Type Not Implemented: {type}")
    };
    
    public static int GetInventorySorterStartIndex(InventoryType type) => type switch
    {
        InventoryType.Inventory1 => 0,
        InventoryType.Inventory2 => GetInventorySorter(type)->ItemsPerPage,
        InventoryType.Inventory3 => GetInventorySorter(type)->ItemsPerPage * 2,
        InventoryType.Inventory4 => GetInventorySorter(type)->ItemsPerPage * 3,
        InventoryType.ArmoryMainHand => 0,
        InventoryType.ArmoryOffHand => 0,
        InventoryType.ArmoryHead => 0,
        InventoryType.ArmoryBody => 0,
        InventoryType.ArmoryHands => 0,
        InventoryType.ArmoryLegs => 0,
        InventoryType.ArmoryFeets => 0,
        InventoryType.ArmoryEar => 0,
        InventoryType.ArmoryNeck => 0,
        InventoryType.ArmoryWrist => 0,
        InventoryType.ArmoryRings => 0,
        _ => throw new Exception($"Type Not Implemented: {type}")
    };

    public static InventoryItem* GetItemForSlot(InventoryType type, int slot) 
        => InventoryManager.Instance()->GetInventoryContainer(type + GetItemOrderDataForSlot(type, slot)->Page)->GetInventorySlot(GetItemOrderDataForSlot(type, slot)->Slot);
}

// Helpers that shouldn't be called directly.
public unsafe partial class InventoryController
{
    protected static StdVector<Pointer<ItemOrderModuleSorterItemEntry>>* GetItemOrderData(InventoryType type)
        => (StdVector<Pointer<ItemOrderModuleSorterItemEntry>>*) &GetInventorySorter(type)->Items;
    
    protected static ItemOrderModuleSorterItemEntry* GetItemOrderDataForSlot(InventoryType type, int slot)
        => GetItemOrderData(type)->Span[slot].Value;
}