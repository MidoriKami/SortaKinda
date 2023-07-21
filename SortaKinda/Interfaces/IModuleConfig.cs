using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Models;

namespace SortaKinda.Interfaces;

public interface IModuleConfig
{
    Dictionary<InventoryType, InventoryConfig>? Configurations { get; set; }
}