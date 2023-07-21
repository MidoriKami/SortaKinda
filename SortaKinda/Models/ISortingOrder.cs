using System.Collections.Generic;
using SortaKinda.Interfaces;

namespace SortaKinda.Models;

public interface ISortingOrder : IComparer<IInventorySlot>
{
    bool CompareSlots(IInventorySlot a, IInventorySlot b);
    void DrawConfigTabs();
    string GetSortingModeString();
}