using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.Models.Configuration;

public class MainInventoryConfig : IModuleConfig {
    public List<InventoryConfig> InventoryConfigs { get; set; } = new() {
        new InventoryConfig(InventoryType.Inventory1),
        new InventoryConfig(InventoryType.Inventory2),
        new InventoryConfig(InventoryType.Inventory3),
        new InventoryConfig(InventoryType.Inventory4)
    };
}