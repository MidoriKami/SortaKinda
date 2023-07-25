using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Enum;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.System.Modules;

public class ArmoryInventoryModule : ModuleBase
{
    public override ModuleName ModuleName { get; protected set; } = ModuleName.ArmoryInventory;
    protected override IModuleConfig ModuleConfig { get; set; } = new ArmoryConfig();
    
    private List<IInventoryGrid>? inventories;
    private ArmoryInventoryGridView? view;
    private readonly Dictionary<InventoryType, int> lastItemCounts = new();

    protected override void Load()
    {
        inventories = new List<IInventoryGrid>();
        foreach (var config in ModuleConfig.InventoryConfigs)
        {
            inventories.Add(new InventoryGrid(config.Type, config));
        }

        view = new ArmoryInventoryGridView(inventories);
    }

    public override void Dispose()
    {
        view?.Dispose();
        base.Dispose();
    }

    public override void Draw()
    {
        view?.Draw();
    }
    
    protected override void Update()
    {
        if (inventories is null) return;
        
        foreach (var inventory in inventories)
        {
            var inventoryCount = InventoryController.GetInventoryItemCount(inventory.Type);

            if (lastItemCounts.TryAdd(inventory.Type, inventoryCount)) continue;

            if (lastItemCounts[inventory.Type] != inventoryCount)
            {
                InventorySorter.SortInventory(inventory.Type, inventory);
                lastItemCounts[inventory.Type] = inventoryCount;
            }
        }
    }
    
    protected override void Sort()
    {
        if (inventories is null) return;

        foreach (var inventory in inventories)
        {
            InventorySorter.SortInventory(inventory.Type, inventory);
        }
    }
}