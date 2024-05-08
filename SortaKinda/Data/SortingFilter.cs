using System;
using SortaKinda.Models.Inventory;

namespace SortaKinda.Models;

public class SortingFilter {
    public required Func<bool> Active { get; init; }
    
    public required Func<InventorySlot, bool> IsSlotAllowed { get; init; }
}