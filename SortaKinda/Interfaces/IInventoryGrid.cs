using System.Collections.Generic;
using System.Numerics;

namespace SortaKinda.Interfaces;

public interface IInventoryGrid
{
    float Scale { get; }
    List<IInventorySlot> InventorySlots { get; set; }
    Vector2 InventorySize { get; }
    void Draw(Vector2 drawPosition);
}