using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Controllers;
using SortaKinda.Modules;

namespace SortaKinda.Classes;

public class InventoryGrid {
    public InventoryGrid(InventoryType type, InventoryConfig config) {
        Type = type;
        Config = config;
        Inventory = [];

        foreach (var index in Enumerable.Range(0, InventoryController.GetInventoryPageSize(Type))) {
            if (Config.SlotConfigs.Count <= index) {
                config.SlotConfigs.Add(new SlotConfig {
                    RuleId = SortController.DefaultId,
                });
                
                Service.Log.Info($"Detected inventory inventory size larger than current config for {type}, updating.");
                System.SortController.SaveConfig();
            }
            
            Inventory.Add(new InventorySlot(Type, config.SlotConfigs[index], index));
        }
    }

    public InventoryConfig Config { get; init; }
    
    public InventoryType Type { get; }
    
    public List<InventorySlot> Inventory { get; set; }
}