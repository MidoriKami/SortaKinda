using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.Interop;

namespace SortaKinda.Classes;

/// <summary>
/// Helper record for processing inventory slots for sorting.
/// </summary>
/// <param name="VisibleSlotIndex">The slot this item appears in.</param>
/// <param name="Item">Pointer to the item.</param>
/// <param name="InventoryType">What InventorySorter Inventory this item is in. (Also known as Adjusted Inventory Type).</param>
public record ItemSlotInfo(int VisibleSlotIndex, Pointer<InventoryItem> Item, InventoryType InventoryType) {
	public void Deconstruct(out int visibleSlotIndex, out Pointer<InventoryItem> item, out InventoryType inventoryType) {
		visibleSlotIndex = VisibleSlotIndex;
		item = Item;
		inventoryType = InventoryType;
	}

	/// <summary>The slot this item appears in.</summary>
	public int VisibleSlotIndex { get; set; } = VisibleSlotIndex;

	/// <summary>Pointer to the item.</summary>
	public Pointer<InventoryItem> Item { get; init; } = Item;

	/// <summary>What InventorySorter Inventory this item is in. (Also known as Adjusted Inventory Type).</summary>
	public InventoryType InventoryType { get; init; } = InventoryType;
}
