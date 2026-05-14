using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace SortaKinda.Extensions;

/// <summary>
/// Extension Methods for getting relevent data from InventoryItems to enable advanced sorting functions.
/// This is where most of the heavy lifting will go for item logic.
/// </summary>
public static unsafe class InventoryItemExtensions {
	extension(ref InventoryItem item) {
		public uint IconId => ItemUtil.GetBaseId(item.ItemId) switch {
			{ Kind: ItemKind.Normal, ItemId: var itemId } => Services.DataManager.GetExcelSheet<Item>().GetRow(itemId).Icon,
			{ Kind: ItemKind.Collectible, ItemId: var itemId } => Services.DataManager.GetExcelSheet<Item>().GetRow(itemId).Icon + 500_000u,
			{ Kind: ItemKind.Hq, ItemId: var itemId } => Services.DataManager.GetExcelSheet<Item>().GetRow(itemId).Icon + 1_000_000u,
			{ Kind: ItemKind.EventItem, ItemId: var itemId } => Services.DataManager.GetExcelSheet<EventItem>().GetRow(itemId).Icon,
			_ => 0,
		};

		public string Name => ItemUtil.GetBaseId(item.ItemId) switch {
			{ Kind: ItemKind.Normal or ItemKind.Collectible or ItemKind.Hq, ItemId: var itemId } => Services.DataManager.GetExcelSheet<Item>().GetRow(itemId).Name.ToString(),
			{ Kind: ItemKind.EventItem, ItemId: var itemId } => Services.DataManager.GetExcelSheet<EventItem>().GetRow(itemId).Name.ToString(),
			_ => string.Empty,
		};
	}
}