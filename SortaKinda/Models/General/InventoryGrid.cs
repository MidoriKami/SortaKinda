using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Inventory;

namespace SortaKinda.System;

public class InventoryGrid : IInventoryGrid
{
    public InventoryType Type { get; }
    public List<IInventorySlot> Inventory { get; set; }
    public InventoryConfig Config { get; init; }

    public InventoryGrid(InventoryType type, InventoryConfig config)
    {
        Type = type;
        Config = config;
        Inventory = new List<IInventorySlot>();
        
        foreach (var index in Enumerable.Range(0, InventoryController.GetInventoryPageSize(Type)))
        {
            Inventory.Add(new InventorySlot(Type, config.SlotConfigs[index], index));
        }
    }
}