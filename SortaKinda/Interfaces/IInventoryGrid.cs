using System.Collections.Generic;

namespace SortaKinda.Interfaces;

public interface IInventoryGrid
{
    List<IInventorySlot> Inventory { get; set; }
}