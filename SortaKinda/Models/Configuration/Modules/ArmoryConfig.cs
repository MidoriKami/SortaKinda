using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.Models.Configuration;

public class ArmoryConfig : IModuleConfig {
    public List<InventoryConfig> InventoryConfigs { get; set; } = new() {
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
        new InventoryConfig(InventoryType.ArmorySoulCrystal)
    };
}