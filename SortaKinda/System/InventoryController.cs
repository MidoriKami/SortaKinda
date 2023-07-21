using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;

namespace SortaKinda.System;

public unsafe partial class InventoryController
{
    public static ItemOrderModuleSorter* GetInventorySorter(InventoryType type)
    {
        if (UIModule.Instance() is null) return null;

        var orderModule = UIModule.Instance()->GetItemOrderModule();
        if (orderModule is null) return null;

        return type switch
        {
            InventoryType.Inventory1 => orderModule->InventorySorter,
            InventoryType.Inventory2 => orderModule->InventorySorter,
            InventoryType.Inventory3 => orderModule->InventorySorter,
            InventoryType.Inventory4 => orderModule->InventorySorter,
            InventoryType.ArmoryMainHand => orderModule->ArmouryMainHandSorter,
            InventoryType.ArmoryOffHand => orderModule->ArmouryOffHandSorter,
            InventoryType.ArmoryHead => orderModule->ArmouryHeadSorter,
            InventoryType.ArmoryBody => orderModule->ArmouryBodySorter,
            InventoryType.ArmoryHands => orderModule->ArmouryHandsSorter,
            InventoryType.ArmoryLegs => orderModule->ArmouryLegsSorter,
            InventoryType.ArmoryFeets => orderModule->ArmouryFeetSorter,
            InventoryType.ArmoryEar => orderModule->ArmouryEarsSorter,
            InventoryType.ArmoryNeck => orderModule->ArmouryNeckSorter,
            InventoryType.ArmoryWrist => orderModule->ArmouryWristsSorter,
            InventoryType.ArmoryRings => orderModule->ArmouryRingsSorter,
            InventoryType.ArmorySoulCrystal => orderModule->ArmourySoulCrystalSorter,
            InventoryType.SaddleBag1 => orderModule->SaddleBagSorter,
            InventoryType.SaddleBag2 => orderModule->PremiumSaddleBagSorter,
            _ => throw new Exception($"Type Not Implemented: {type}")
        };
    }

    public static int GetInventorySorterStartIndex(InventoryType type)
    {
        var itemSorter = GetInventorySorter(type);
        if (itemSorter is null) return 0;

        return type switch
        {
            InventoryType.Inventory2 => itemSorter->ItemsPerPage,
            InventoryType.Inventory3 => itemSorter->ItemsPerPage * 2,
            InventoryType.Inventory4 => itemSorter->ItemsPerPage * 3,
            _ => 0
        };
    }

    public static InventoryItem* GetItemForSlot(InventoryType type, int slot)
    {
        var inventoryManager = InventoryManager.Instance();
        if (inventoryManager is null) return null;

        var itemOrderData = GetItemOrderDataForSlot(type, slot).Value;
        if (itemOrderData is null) return null;

        var inventoryContainer = inventoryManager->GetInventoryContainer(GetAdjustedInventoryType(type) + itemOrderData->Page);
        if (inventoryContainer is null) return null;

        return inventoryContainer->GetInventorySlot(itemOrderData->Slot);
    }

    public static Pointer<ItemOrderModuleSorterItemEntry> GetItemOrderDataForSlot(InventoryType type, int slot)
    {
        var itemOrderData = GetItemOrderData(type);
        if (itemOrderData is null) return null;

        return GetItemOrderData(type)->Span[slot + GetInventorySorterStartIndex(type)];
    }

    public static int GetInventoryItemCount(params InventoryType[] types)
    {
        return types.Sum(GetInventoryItemCount);
    }

    public static int GetInventoryItemCount(InventoryType type)
    {
        var count = 0;
        foreach (var item in GetInventoryItems(type))
        {
            if (item is not { ItemID: 0 }) count++;
        }

        return count;
    }
}

// Helpers that shouldn't be called directly.
public unsafe partial class InventoryController
{
    private static InventoryType GetAdjustedInventoryType(InventoryType type)
    {
        return type switch
        {
            InventoryType.Inventory1 => InventoryType.Inventory1,
            InventoryType.Inventory2 => InventoryType.Inventory1,
            InventoryType.Inventory3 => InventoryType.Inventory1,
            InventoryType.Inventory4 => InventoryType.Inventory1,
            _ => type
        };
    }

    private static Span<InventoryItem> GetInventoryItems(InventoryType type)
    {
        var instance = InventoryManager.Instance();
        if (instance is null) return Span<InventoryItem>.Empty;

        var container = instance->GetInventoryContainer(type);
        if (container is null) return Span<InventoryItem>.Empty;

        return new Span<InventoryItem>(container->Items, (int) container->Size);
    }

    private static StdVector<Pointer<ItemOrderModuleSorterItemEntry>>* GetItemOrderData(InventoryType type)
    {
        var inventorySorter = GetInventorySorter(type);
        if (inventorySorter is null) return null;

        return &inventorySorter->Items;
    }
}