using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Enums;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.System.Modules;

public class ArmoryInventoryModule : ModuleBase {
    private List<IInventoryGrid>? inventories;
    private ArmoryInventoryGridView? view;
    public override ModuleName ModuleName => ModuleName.ArmoryInventory;
    protected override IModuleConfig ModuleConfig { get; set; } = new ArmoryConfig();

    protected override void LoadViews() {
        inventories = new List<IInventoryGrid>();
        foreach (var config in ModuleConfig.InventoryConfigs) {
            inventories.Add(new InventoryGrid(config.Type, config));
        }

        view = new ArmoryInventoryGridView(inventories);
    }

    public override void Dispose() {
        view?.Dispose();
        base.Dispose();
    }

    public override void Draw() {
        view?.Draw();
    }

    protected override void InventoryChanged(InventoryType type) {
        var targetInventory = inventories?.FirstOrDefault(inventory => inventory.Type == type);
        if (targetInventory is null) return;

        SortaKindaController.SortingThreadController.AddSortingTask(type, targetInventory);
    }

    protected override void Sort() {
        if (inventories is null) return;

        foreach (var inventory in inventories) {
            SortaKindaController.SortingThreadController.AddSortingTask(inventory.Type, inventory);
        }
    }
}