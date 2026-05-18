using System;
using System.Linq;
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

		public ItemUICategory UiCategory
			=> item.GetItemProperty(itemData => itemData.ItemUICategory.Value);

		public uint ItemLevel
			=> item.GetItemProperty(itemData => itemData.LevelItem.RowId);

		public uint ItemRarity
			=> item.GetItemProperty(itemData => itemData.Rarity);

		public uint SellPrice
			=> item.GetItemProperty(itemData => itemData.PriceLow);

		public bool IsDyeable
			=> item.GetItemProperty(itemData => itemData.DyeCount > 0);

		public uint EquipLevel
			=> item.GetItemProperty(itemData => itemData.LevelEquip);

		public bool IsRepairable
			=> item.GetItemProperty(itemData => itemData.ItemRepair.RowId is not 0);

		public bool IsUntradable
			=> item.GetItemProperty(itemData => itemData.IsUntradable);

		public bool IsUnique
			=> item.GetItemProperty(itemData => itemData.IsUnique);

		private T GetItemProperty<T>(Func<Item, T> propertyGetter) {
			if (!ItemUtil.IsNormalItem(item.ItemId)) throw new Exception("Invalid Item Type");

			return propertyGetter(Services.DataManager.GetExcelSheet<Item>().GetRow(item.ItemId));
		}
	}
}