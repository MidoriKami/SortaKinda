using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Interfaces;

namespace SortaKinda.Models;

public class GenericModuleConfig : IModuleConfig
{
    public Dictionary<InventoryType, InventoryConfig>? Configurations { get; set; }
}