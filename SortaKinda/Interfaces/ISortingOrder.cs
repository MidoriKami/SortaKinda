using System.Collections.Generic;

namespace SortaKinda.Interfaces;

public interface ISortingOrder : IComparer<IInventorySlot>
{
    bool CompareSlots(IInventorySlot a, IInventorySlot b);
    void DrawConfigTabs();
    string GetSortingModeString();
}