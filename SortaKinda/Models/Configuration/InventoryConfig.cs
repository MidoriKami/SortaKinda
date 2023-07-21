using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.System;

namespace SortaKinda.Models;

public unsafe class InventoryConfig
{
    public InventoryConfig(InventoryType type)
    {
        Type = type;
        var ruleCount = InventoryController.GetInventorySorter(type)->ItemsPerPage;

        Rules = new string[ruleCount];
        foreach (var index in Enumerable.Range(0, ruleCount))
        {
            Rules[index] = "Default";
        }
    }

    public InventoryType Type { get; set; }
    public string[] Rules { get; set; }
}