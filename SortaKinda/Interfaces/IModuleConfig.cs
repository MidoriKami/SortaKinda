using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.Interfaces;

public interface IModuleConfig
{
    Dictionary<InventoryType, IInventoryConfig>? Configurations { get; set; }
}