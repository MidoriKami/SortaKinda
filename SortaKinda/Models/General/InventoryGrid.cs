using System.Collections.Generic;
using System.Linq;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Inventory;

namespace SortaKinda.System;

public class InventoryGrid : IInventoryGrid
{
    public InventoryGrid(InventoryType type, InventoryConfig config)
    {
        Type = type;
        Config = config;
        Inventory = new List<IInventorySlot>();

        PluginLog.Debug(Type.ToString());

        foreach (var index in Enumerable.Range(0, InventoryController.GetInventoryPageSize(Type)))
        {
            Inventory.Add(new InventorySlot(Type, config.SlotConfigs[index], index));
        }
    }

    public InventoryConfig Config { get; init; }
    public InventoryType Type { get; }
    public List<IInventorySlot> Inventory { get; set; }
}