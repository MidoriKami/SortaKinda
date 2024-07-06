using System;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SortaKinda.Controllers;

public static unsafe partial class InventoryController {
    public static int GetInventoryPageSize(InventoryType type)
        => GetInventorySorter(type)->ItemsPerPage;

    public static InventoryItem* GetItemForSlot(InventoryType type, int slot)
        => InventoryManager.Instance()->GetInventoryContainer(GetAdjustedInventoryType(type) + GetItemOrderData(type, slot)->Page)->GetInventorySlot(GetItemOrderData(type, slot)->Slot);

    public static ItemOrderModuleSorterItemEntry* GetItemOrderData(InventoryType type, int slot) 
        => GetInventorySorter(type)->Items[slot + GetInventoryStartIndex(type)];
    
        private static ItemOrderModuleSorter* GetInventorySorter(InventoryType type) => type switch {
        InventoryType.Inventory1 => ItemOrderModule.Instance()->InventorySorter,
        InventoryType.Inventory2 => ItemOrderModule.Instance()->InventorySorter,
        InventoryType.Inventory3 => ItemOrderModule.Instance()->InventorySorter,
        InventoryType.Inventory4 => ItemOrderModule.Instance()->InventorySorter,
        InventoryType.ArmoryMainHand => ItemOrderModule.Instance()->ArmouryMainHandSorter,
        InventoryType.ArmoryOffHand => ItemOrderModule.Instance()->ArmouryOffHandSorter,
        InventoryType.ArmoryHead => ItemOrderModule.Instance()->ArmouryHeadSorter,
        InventoryType.ArmoryBody => ItemOrderModule.Instance()->ArmouryBodySorter,
        InventoryType.ArmoryHands => ItemOrderModule.Instance()->ArmouryHandsSorter,
        InventoryType.ArmoryLegs => ItemOrderModule.Instance()->ArmouryLegsSorter,
        InventoryType.ArmoryFeets => ItemOrderModule.Instance()->ArmouryFeetSorter,
        InventoryType.ArmoryEar => ItemOrderModule.Instance()->ArmouryEarsSorter,
        InventoryType.ArmoryNeck => ItemOrderModule.Instance()->ArmouryNeckSorter,
        InventoryType.ArmoryWrist => ItemOrderModule.Instance()->ArmouryWristsSorter,
        InventoryType.ArmoryRings => ItemOrderModule.Instance()->ArmouryRingsSorter,
        InventoryType.ArmorySoulCrystal => ItemOrderModule.Instance()->ArmourySoulCrystalSorter,
        _ => throw new Exception($"Type Not Implemented: {type}"),
    };

    private static InventoryType GetAdjustedInventoryType(InventoryType type) => type switch {
        InventoryType.Inventory1 => InventoryType.Inventory1,
        InventoryType.Inventory2 => InventoryType.Inventory1,
        InventoryType.Inventory3 => InventoryType.Inventory1,
        InventoryType.Inventory4 => InventoryType.Inventory1,
        _ => type,
    };

    private static int GetInventoryStartIndex(InventoryType type) => type switch {
        InventoryType.Inventory2 => GetInventorySorter(type)->ItemsPerPage,
        InventoryType.Inventory3 => GetInventorySorter(type)->ItemsPerPage * 2,
        InventoryType.Inventory4 => GetInventorySorter(type)->ItemsPerPage * 3,
        _ => 0,
    };
}