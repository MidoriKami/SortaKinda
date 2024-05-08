using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Configuration;
using SortaKinda.Controllers;
using SortaKinda.Data;
using SortaKinda.Data.Enums;
using SortaKinda.Windows.ConfigurationWindow.cs.Components;

namespace SortaKinda.Modules;

public class MainInventoryConfig : IModuleConfig {
    public List<InventoryConfig> InventoryConfigs { get; set; } = [
        new InventoryConfig(InventoryType.Inventory1),
        new InventoryConfig(InventoryType.Inventory2),
        new InventoryConfig(InventoryType.Inventory3),
        new InventoryConfig(InventoryType.Inventory4)
    ];
}

public class MainInventoryModule : ModuleBase<MainInventoryConfig> {
    private QuadInventoryView? view;
    
    public override ModuleName ModuleName => ModuleName.MainInventory;
    
    protected override List<InventoryGrid> Inventories { get; set; } = null!;
    
    protected override MainInventoryConfig ModuleConfig { get; set; } = new();

    public override void Draw() {
        view?.Draw();
    }
    
    protected override void LoadViews() {
        Inventories = [];
        foreach (var config in ModuleConfig.InventoryConfigs) {
            Inventories.Add(new InventoryGrid(config.Type, config));
        }

        view = new QuadInventoryView(Inventories, Vector2.Zero);
    } 

    protected override void Sort(params InventoryType[] inventoryTypes) {
        if (Inventories.SelectMany(inventory => inventory.Inventory).Any(slot => slot.Rule.Id is not SortController.DefaultId)) {
            SortaKindaController.SortingThreadController.AddSortingTask(InventoryType.Inventory1, Inventories.ToArray());
        }
    }
}