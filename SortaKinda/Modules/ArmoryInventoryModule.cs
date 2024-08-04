using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Classes;
using SortaKinda.Controllers;
using SortaKinda.ViewComponents;

namespace SortaKinda.Modules;

public class ArmoryConfig : IModuleConfig {
    public List<InventoryConfig> InventoryConfigs { get; set; } = [
        new InventoryConfig(InventoryType.ArmoryMainHand),
        new InventoryConfig(InventoryType.ArmoryOffHand),
        new InventoryConfig(InventoryType.ArmoryHead),
        new InventoryConfig(InventoryType.ArmoryBody),
        new InventoryConfig(InventoryType.ArmoryHands),
        new InventoryConfig(InventoryType.ArmoryLegs),
        new InventoryConfig(InventoryType.ArmoryFeets),
        new InventoryConfig(InventoryType.ArmoryEar),
        new InventoryConfig(InventoryType.ArmoryNeck),
        new InventoryConfig(InventoryType.ArmoryWrist),
        new InventoryConfig(InventoryType.ArmoryRings),
        new InventoryConfig(InventoryType.ArmorySoulCrystal),
    ];
}

public class ArmoryInventoryModule : ModuleBase<ArmoryConfig> {
    protected override List<InventoryGrid> Inventories { get; set; } = null!;
    
    private ArmoryInventoryGridView? view;
    
    public override ModuleName ModuleName => ModuleName.ArmoryInventory;
    
    public override ArmoryConfig ModuleConfig { get; set; } = new();

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
                    System.SortingThreadController.AddSortingTask(targetInventory.Type, targetInventory);
                }
            }
        }
    }
}