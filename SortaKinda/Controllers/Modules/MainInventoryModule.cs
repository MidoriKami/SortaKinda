using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Enums;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.System.Modules;

public class MainInventoryModule : ModuleBase {
    private QuadInventoryView? view;
    public override ModuleName ModuleName => ModuleName.MainInventory;
    protected override List<IInventoryGrid> Inventories { get; set; } = null!;
    protected override IModuleConfig ModuleConfig { get; set; } = new MainInventoryConfig();

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