using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SortaKinda.System;

public unsafe partial class InventoryController
{
    public static int GetInventoryPageSize(InventoryType type)
    {
        return GetInventorySorter(type)->ItemsPerPage;
    }

    public static InventoryItem* GetItemForSlot(InventoryType type, int slot)
    {
        return InventoryManager.Instance()->GetInventoryContainer(GetAdjustedInventoryType(type) + GetItemOrderData(type, slot)->Page)
            ->GetInventorySlot(GetItemOrderData(type, slot)->Slot);
    }

    public static ItemOrderModuleSorterItemEntry* GetItemOrderData(InventoryType type, int slot)
    {
        return GetInventorySorter(type)->Items.Span[slot + GetInventoryStartIndex(type)];
    }

    public static int GetInventoryItemCount(params InventoryType[] types)
    {
        return types.Sum(GetInventoryItemCount);
    }
}

// Helper Methods
public unsafe partial class InventoryController
{
    private static ItemOrderModuleSorter* GetInventorySorter(InventoryType type)
    {
        return type switch
        {
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
            InventoryType.SaddleBag1 => ItemOrderModule.Instance()->SaddleBagSorter,
            InventoryType.SaddleBag2 => ItemOrderModule.Instance()->PremiumSaddleBagSorter,
            _ => throw new Exception($"Type Not Implemented: {type}")
        };
    }

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

    private static int GetInventoryStartIndex(InventoryType type)
    {
        return type switch
        {
            InventoryType.Inventory2 => GetInventorySorter(type)->ItemsPerPage,
            InventoryType.Inventory3 => GetInventorySorter(type)->ItemsPerPage * 2,
            InventoryType.Inventory4 => GetInventorySorter(type)->ItemsPerPage * 3,
            _ => 0
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

    private static int GetInventoryItemCount(InventoryType type)
    {
        var count = 0;
        foreach (var item in GetInventoryItems(type))
        {
            if (item is not { ItemID: 0 }) count++;
        }

        return count;
    }
}