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
        var inventorySize = InventoryController.GetInventorySize(type);

        SlotConfigs = new List<SlotConfig>();
        foreach (var _ in Enumerable.Range(0, inventorySize))
        {
            SlotConfigs.Add(new SlotConfig
            {
                Type = Type,
                RuleId = SortController.DefaultId,
                NeedsSaving = false,
            });
        }
    }

    public InventoryType Type { get; set; }
    public List<SlotConfig> SlotConfigs { get; set; }
    public bool NeedsSaving { get; set; }
}