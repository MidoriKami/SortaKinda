using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.System;

namespace SortaKinda.Models;

public unsafe class InventoryConfig
{
    public InventoryType Type { get; set; }
    public SortingRule[] Rules { get; set; }
    
    public InventoryConfig(InventoryType type)
    {
        Type = type;
        var ruleCount = InventoryController.GetInventorySorter(type)->ItemsPerPage;

        Rules = new SortingRule[ruleCount];
        foreach (var index in Enumerable.Range(0, ruleCount))
        {
            Rules[index] = new SortingRule
            {
                Id = string.Empty,
            };
        }
    }
}