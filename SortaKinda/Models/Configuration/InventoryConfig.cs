using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.System;

namespace SortaKinda.Models.Configuration;

public class InventoryConfig
{
    public InventoryConfig(InventoryType type)
    {
        Type = type;
        SlotConfigs = new List<SlotConfig>();
        foreach (var _ in Enumerable.Range(0, InventoryController.GetInventoryPageSize(type)))
        {
            SlotConfigs.Add(new SlotConfig
            {
                RuleId = SortController.DefaultId,
            });
        }
    }

    public List<SlotConfig> SlotConfigs { get; set; }
    public InventoryType Type { get; set; }
}