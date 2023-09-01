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

    protected override void InventoryChanged(InventoryType type)
    {
        if (type is InventoryType.Inventory1)
        {
            Sort();
        }
    }

    protected override void Sort()
    {
        if (inventories is null) return;

        SortaKindaController.SortingThreadController.AddSortingTask(InventoryType.Inventory1, inventories.ToArray());
    }
}