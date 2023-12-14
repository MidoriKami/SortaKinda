using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Enums;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.System.Modules;

public class ArmoryInventoryModule : ModuleBase {
    protected override List<IInventoryGrid> Inventories { get; set; } = null!;
    private ArmoryInventoryGridView? view;
    public override ModuleName ModuleName => ModuleName.ArmoryInventory;
    protected override IModuleConfig ModuleConfig { get; set; } = new ArmoryConfig();

    protected override void LoadViews() {
        Inventories = [];
        foreach (var config in ModuleConfig.InventoryConfigs) {
            Inventories.Add(new InventoryGrid(config.Type, config));
        }

        view = new ArmoryInventoryGridView(Inventories);
    }

    public override void Dispose() {
        view?.Dispose();
        base.Dispose();
    }

    public override void Draw() {
        view?.Draw();
    }

    protected override void Sort(params InventoryType[] inventoryTypes) {
        foreach (var type in inventoryTypes) {
            if (Inventories.FirstOrDefault(inventory => inventory.Type == type) is { } targetInventory) {
                if (targetInventory.Inventory.Any(slot => slot.Rule.Id is not SortController.DefaultId)) {
                    SortaKindaController.SortingThreadController.AddSortingTask(targetInventory.Type, targetInventory);
                }
            }
        }
    }
}