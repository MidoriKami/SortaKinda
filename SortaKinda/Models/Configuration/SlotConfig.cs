using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.System;

namespace SortaKinda.Models.Configuration;

public class SlotConfig
{
    public InventoryType Type { get; set; }
    public string RuleId { get; set; } = SortController.DefaultId;
    public bool NeedsSaving { get; set; }
}