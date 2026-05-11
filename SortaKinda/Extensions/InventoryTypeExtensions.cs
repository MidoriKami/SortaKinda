using System;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SortaKinda.Extensions;

/// <summary>
/// Extensions for InventoryType enum, to get items and information based on which inventory is being used.
/// </summary>
public static unsafe class InventoryTypeExtensions {
	extension(InventoryType inventory) {
		/// <summary>
		/// Get the item stored in the specified visual slot.
		/// </summary>
		/// <param name="slot">Which visual slot to return.</param>
		/// <returns>The inventory item in that slot, will always be non-null, but will have invalid ItemId if empty.</returns>
		public InventoryItem* GetItem(int slot)
			=> InventoryManager.Instance()->GetInventoryContainer(
				inventory.AdjustedInventoryType + inventory.GetItemOrderData(slot)->Page)
				->GetInventorySlot(inventory.GetItemOrderData(slot)->Slot);

		public string Name => inventory switch {
			InventoryType.Inventory1 => "Inventory 1",
			InventoryType.Inventory2 => "Inventory 2",
			InventoryType.Inventory3 => "Inventory 3",
			InventoryType.Inventory4 => "Inventory 4",
			InventoryType.ArmoryMainHand => "Main Hand",
			InventoryType.ArmoryOffHand => "Off Hand",
			InventoryType.ArmoryHead => "Head",
			InventoryType.ArmoryBody => "Body",
			InventoryType.ArmoryHands => "Hands",
			InventoryType.ArmoryLegs => "Legs",
			InventoryType.ArmoryFeets => "Feet",
			InventoryType.ArmoryNeck => "Neck",
			InventoryType.ArmoryEar => "Ears",
			InventoryType.ArmoryWrist => "Wrists",
			InventoryType.ArmoryRings => "Rings",
			InventoryType.ArmorySoulCrystal => "Soul Crystal",
			_ => inventory.ToString(),
		};

		/// <summary>
		/// Get the item entry for a specific slot, to find what item it's actually referring to.
		/// </summary>
		/// <param name="slot"></param>
		/// <returns></returns>
		private ItemOrderModuleSorterItemEntry* GetItemOrderData(int slot)
			=> inventory.InventorySorter->Items[slot + inventory.SlotStartIndex];

		/// <summary>
		/// How many items there are per page for this inventories sorter.
		/// </summary>
		public int ItemsPerPage
			=> inventory.InventorySorter->ItemsPerPage;

		/// <summary>
		/// Gets the Inventory Sorter for this Inventory.
		/// </summary>
		/// <remarks>
		/// The main inventory only has a single InventorySorter,
		/// so we redirect Inventory 2/3/4 to the main Inventory Sorter.
		/// </remarks>
		public ItemOrderModuleSorter* InventorySorter => inventory switch {
			InventoryType.Inventory1 => ItemOrderModule.Instance()->InventorySorter,
			InventoryType.Inventory2 => ItemOrderModule.Instance()->InventorySorter,
			InventoryType.Inventory3 => ItemOrderModule.Instance()->InventorySorter,
			InventoryType.Inventory4 => ItemOrderModule.Instance()->InventorySorter,
			InventoryType.ArmoryMainHand => ItemOrderModule.Instance()->ArmouryMainHandSorter,
			InventoryType.ArmoryOffHand => ItemOrderModule.Instance()->ArmouryOffHandSorter,
			InventoryType.ArmoryHead => ItemOrderModule.Instance()->ArmouryHeadSorter,
			InventoryType.ArmoryBody => ItemOrderModule.Instance()->ArmouryBodySorter,
			InventoryType.ArmoryHands => ItemOrderModule.Instance()->ArmouryHandsSorter,
			InventoryType.ArmoryLegs => ItemOrderModule.Instance()->ArmouryLegsSorter,
			InventoryType.ArmoryFeets => ItemOrderModule.Instance()->ArmouryFeetSorter,
			InventoryType.ArmoryEar => ItemOrderModule.Instance()->ArmouryEarsSorter,
			InventoryType.ArmoryNeck => ItemOrderModule.Instance()->ArmouryNeckSorter,
			InventoryType.ArmoryWrist => ItemOrderModule.Instance()->ArmouryWristsSorter,
			InventoryType.ArmoryRings => ItemOrderModule.Instance()->ArmouryRingsSorter,
			InventoryType.ArmorySoulCrystal => ItemOrderModule.Instance()->ArmourySoulCrystalSorter,
			_ => throw new Exception($"Type Not Implemented: {inventory}"),
		};

		/// <summary>
		/// Gets the starting index for this Inventory, only Inventory2/3/4 will offset the starting index.
		/// </summary>
		private int SlotStartIndex => inventory switch {
			InventoryType.Inventory2 => inventory.InventorySorter->ItemsPerPage,
			InventoryType.Inventory3 => inventory.InventorySorter->ItemsPerPage * 2,
			InventoryType.Inventory4 => inventory.InventorySorter->ItemsPerPage * 3,
			_ => 0,
		};

		/// <summary>
		/// Gets the adjusted inventory for this Inventory, only Inventory2/3/4 will be redirected.
		/// </summary>
		private InventoryType AdjustedInventoryType => inventory switch {
			InventoryType.Inventory1 => InventoryType.Inventory1,
			InventoryType.Inventory2 => InventoryType.Inventory1,
			InventoryType.Inventory3 => InventoryType.Inventory1,
			InventoryType.Inventory4 => InventoryType.Inventory1,
			_ => inventory,
		};
	}
}