using System;
using SortaKinda.Interfaces;

namespace SortaKinda.Models;

public class SortingFilter
{
    public required Func<bool> Active { get; init; }
    public required Func<IInventorySlot, bool> IsSlotAllowed { get; init; }
}