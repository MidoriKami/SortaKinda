using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Abstracts;

namespace SortaKinda.Models;

public class MainModuleConfig : IModuleConfig
{
    public Dictionary<InventoryType, InventoryConfig>? Configurations { get; set; } = null;
}