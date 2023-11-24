using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Models.Enums;
using SortaKinda.Models.General;

namespace SortaKinda.System;

public unsafe class InventoryScanner
{
    private readonly Dictionary<InventoryType, Dictionary<int, uint>> inventoryCache = new();
    
    private readonly InventoryType[] inventories = 
    {
        InventoryType.Inventory1,
        InventoryType.ArmoryMainHand,
        InventoryType.ArmoryOffHand,
        InventoryType.ArmoryHead,
        InventoryType.ArmoryBody,
        InventoryType.ArmoryHands,
        InventoryType.ArmoryLegs,
        InventoryType.ArmoryFeets,
        InventoryType.ArmoryEar,
        InventoryType.ArmoryNeck,
        InventoryType.ArmoryWrist,
        InventoryType.ArmoryRings,
        InventoryType.ArmorySoulCrystal,
    };

    public event Action<InventoryType>? InventoryChanged;
    
    public InventoryScanner()
    {
        void InitializeCache(InventoryType type)
        {
            inventoryCache.Add(type, new Dictionary<int, uint>());

            foreach (var item in GetItems(type))
            {
                inventoryCache[type].Add(item.Slot, item.ItemID);
            }
        }
        
        foreach(var type in inventories) InitializeCache(type);
        InitializeCache(InventoryType.Inventory2);
        InitializeCache(InventoryType.Inventory3);
        InitializeCache(InventoryType.Inventory4);
        
        InventoryChanged += type =>
        {
            Service.Log.Verbose($"Inventory Changed: {type}");
        };
    }

    public void Update()
    {
        if (!SortaKindaController.SystemConfig.SortOnInventoryChange) return;

        foreach (var inventory in inventories)
        {
            var changes = inventory is InventoryType.Inventory1 ? GroupInventoryScan(InventoryType.Inventory1, InventoryType.Inventory2, InventoryType.Inventory3, InventoryType.Inventory4) : IndividualInventoryScan(inventory);

            if (Normalize(changes).Any())
            {
                InventoryChanged?.Invoke(inventory);
            }
        }
    }

    private List<ItemChangelog> GroupInventoryScan(params InventoryType[] types) 
        => types.SelectMany(IndividualInventoryScan).ToList();

    private List<ItemChangelog> IndividualInventoryScan(InventoryType type)
    {
        var changelog = new List<ItemChangelog>();

        foreach (var item in GetItems(type))
        {
            // Gained item or item changed
            if (inventoryCache[type][item.Slot] == 0 && item.ItemID != 0 || (inventoryCache[type][item.Slot] != item.ItemID && item.ItemID != 0))
            {
                changelog.Add(new ItemChangelog(ChangelogState.Added, item.ItemID));
                Service.Log.Verbose($"[InventoryScanner] Adding - {item.ItemID}");
            }
            // Lost item
            else if (inventoryCache[type][item.Slot] != 0 && item.ItemID == 0)
            {
                changelog.Add(new ItemChangelog(ChangelogState.Removed, inventoryCache[type][item.Slot]));
                Service.Log.Verbose($"[InventoryScanner] Removing - {item.ItemID}");
            }

            inventoryCache[type][item.Slot] = item.ItemID;
        }

        return changelog;
    }

    private ReadOnlySpan<InventoryItem> GetItems(InventoryType type)
    {
        var inventoryManager = InventoryManager.Instance();
        if (inventoryManager is null) return Span<InventoryItem>.Empty;

        var inventory = inventoryManager->GetInventoryContainer(type);
        if (inventory is null) return Span<InventoryItem>.Empty;

        return new ReadOnlySpan<InventoryItem>(inventory->Items, (int)inventory->Size);
    }

    private static IEnumerable<ItemChangelog> Normalize(IEnumerable<ItemChangelog> changelogs)
    {
        var result = new List<ItemChangelog>();
        
        foreach (var itemGroup in changelogs.GroupBy(log => log.ItemId))
        {
            var hasAdd = false;
            var hasRemove = false;
            
            foreach (var log in itemGroup)
            {
                if (log.State is ChangelogState.Added) hasAdd = true;
                if (log.State is ChangelogState.Removed) hasRemove = true;
            }

            var itemMoved = hasAdd && hasRemove;
            var itemRemoved = !hasAdd && hasRemove;
            var itemAdded = hasAdd && !hasRemove;

            if (!itemMoved)
            {
                if (itemAdded) result.Add(new ItemChangelog(ChangelogState.Added, itemGroup.Key));
                if (itemRemoved) result.Add(new ItemChangelog(ChangelogState.Removed, itemGroup.Key));
            }
        }

        return result;
    }
}