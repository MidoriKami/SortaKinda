using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;

namespace SortaKinda.Extensions;

/// <summary>
/// Extension Methods for getting relevent data from InventoryItems to enable advanced sorting functions.
/// This is where most of the heavy lifting will go for item logic.
/// </summary>
public static unsafe class InventoryItemExtensions {
	extension(ref InventoryItem item) {
		public bool IsInGearset
			=> item.GetIsInGearset();

		public uint IconId
			=> item.GetIconId();

		private bool GetIsInGearset() {
			if (item.GetItemId() is 0) return false;

			foreach (var enabledGearsetIndex in RaptureGearsetModule.Instance()->EnabledGearsetIndex2EntryIndex) {
				if (enabledGearsetIndex is 0) continue;

				foreach (ref var itemInGearset in RaptureGearsetModule.Instance()->Entries[enabledGearsetIndex].Items) {
					if (itemInGearset.ItemId == item.GetItemId()) return true;
				}
			}

			return false;
		}

		private uint GetIconId() => ItemUtil.GetBaseId(item.ItemId) switch {
			{ Kind: ItemKind.Normal, ItemId: var itemId } => Services.DataManager.GetExcelSheet<Item>().GetRow(itemId).Icon,
			{ Kind: ItemKind.Collectible, ItemId: var itemId } => Services.DataManager.GetExcelSheet<Item>().GetRow(itemId).Icon + 500_000u,
			{ Kind: ItemKind.Hq, ItemId: var itemId } => Services.DataManager.GetExcelSheet<Item>().GetRow(itemId).Icon + 1_000_000u,
			{ Kind: ItemKind.EventItem, ItemId: var itemId } => Services.DataManager.GetExcelSheet<EventItem>().GetRow(itemId).Icon,
			_ => 0,
		};
	}
}