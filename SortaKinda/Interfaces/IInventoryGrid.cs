using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Models.Configuration;

namespace SortaKinda.Interfaces;

public interface IInventoryGrid
{
    List<IInventorySlot> Inventory { get; set; }
    InventoryConfig Config { get; init; }

    void Update();
}