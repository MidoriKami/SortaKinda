using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.Interfaces;

public interface IInventoryGrid
{
    List<IInventorySlot> Inventory { get; set; }
    InventoryType Type { get; }
}