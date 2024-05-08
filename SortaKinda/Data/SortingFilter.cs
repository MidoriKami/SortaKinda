using System;

namespace SortaKinda.Data;

public class SortingFilter {
    public required Func<bool> Active { get; init; }
    
    public required Func<InventorySlot, bool> IsSlotAllowed { get; init; }
}