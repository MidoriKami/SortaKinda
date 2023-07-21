namespace SortaKinda.Interfaces;

public interface ISortingFilter
{
    bool IsItemSlotAllowed(IInventorySlot slot);
    void DrawConfigTabs();
    string GetAllowedItemsString();
}