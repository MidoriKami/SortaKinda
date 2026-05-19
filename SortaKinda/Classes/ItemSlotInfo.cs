using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.Interop;

namespace SortaKinda.Classes;

/// <summary>
/// Helper record for processing inventory slots for sorting.
/// </summary>
/// <param name="VisibleSlotIndex">The slot this item appears in.</param>
/// <param name="Item">Pointer to the item.</param>
/// <param name="InventoryType">What InventorySorter Inventory this item is in. (Also known as Adjusted Inventory Type).</param>
public record struct ItemSlotInfo(int VisibleSlotIndex, Pointer<InventoryItem> Item, InventoryType InventoryType);
