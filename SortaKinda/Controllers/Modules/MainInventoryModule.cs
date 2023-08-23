using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Enums;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.System.Modules;

public class MainInventoryModule : ModuleBase
{
    private List<IInventoryGrid>? inventories;
    private long mainInventoryLastCount = long.MaxValue;
    private QuadInventoryView? view;
    public override ModuleName ModuleName => ModuleName.MainInventory;
    protected override IModuleConfig ModuleConfig { get; set; } = new MainInventoryConfig();

    public override void Draw()
    {
        view?.Draw();
    }
    
    protected override void LoadViews()
    {
        inventories = new List<IInventoryGrid>();
        foreach (var config in ModuleConfig.InventoryConfigs)
        {
            inventories.Add(new InventoryGrid(config.Type, config));
        }

        view = new QuadInventoryView(inventories, Vector2.Zero);
    }

    protected override void Update()
    {
        var currentInventoryCount = InventoryController.GetInventoryItemCount(InventoryType.Inventory1, InventoryType.Inventory2, InventoryType.Inventory3, InventoryType.Inventory4);

        if (mainInventoryLastCount is long.MaxValue) mainInventoryLastCount = currentInventoryCount;
        
        if (mainInventoryLastCount != currentInventoryCount)
        {
            if (SortaKindaController.SystemConfig.SortOnInventoryChange) Sort();
            mainInventoryLastCount = currentInventoryCount;
        }
    }

    protected override void Sort()
    {
        if (inventories is null) return;

        SortaKindaController.SortingThreadController.AddSortingTask(InventoryType.Inventory1, inventories.ToArray());
    }
}