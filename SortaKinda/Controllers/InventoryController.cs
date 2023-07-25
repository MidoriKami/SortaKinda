using System;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SortaKinda.System;

public unsafe partial class InventoryController
{
    public static int GetInventoryPageSize(InventoryType type) 
        => GetInventorySorter(type)->ItemsPerPage;
    
    public static int GetInventoryStartIndex(InventoryType type) => type switch
    {
        InventoryType.Inventory2 => GetInventorySorter(type)->ItemsPerPage,
        InventoryType.Inventory3 => GetInventorySorter(type)->ItemsPerPage * 2,
        InventoryType.Inventory4 => GetInventorySorter(type)->ItemsPerPage * 3,
        _ => 0
    };

    public static InventoryItem* GetItemForSlot(InventoryType type, int slot)
        => InventoryManager.Instance()->GetInventoryContainer(GetAdjustedInventoryType(type) + GetItemOrderData(type, slot)->Page)
            ->GetInventorySlot(GetItemOrderData(type, slot)->Slot);

    public static ItemOrderModuleSorterItemEntry* GetItemOrderData(InventoryType type, int slot)
        => GetInventorySorter(type)->Items.Span[slot + GetInventoryStartIndex(type)];
}

// Helper Methods
public unsafe partial class InventoryController
{
    private static ItemOrderModuleSorter* GetInventorySorter(InventoryType type) => type switch
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
        InventoryType.ArmorySoulCrystal => UIModule.Instance()->GetItemOrderModule()->ArmourySoulCrystalSorter,
        InventoryType.SaddleBag1 => UIModule.Instance()->GetItemOrderModule()->SaddleBagSorter,
        InventoryType.SaddleBag2 => UIModule.Instance()->GetItemOrderModule()->PremiumSaddleBagSorter,
        _ => throw new Exception($"Type Not Implemented: {type}")
    };
    
    private static InventoryType GetAdjustedInventoryType(InventoryType type) => type switch
    {
        InventoryType.Inventory1 => InventoryType.Inventory1,
        InventoryType.Inventory2 => InventoryType.Inventory1,
        InventoryType.Inventory3 => InventoryType.Inventory1,
        InventoryType.Inventory4 => InventoryType.Inventory1,
        _ => type
    };
}