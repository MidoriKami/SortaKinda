using System.Linq;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Configuration;
using SortaKinda.Extensions;
using SortaKinda.Windows.UiParts;

namespace SortaKinda.Classes;

/// <summary>
/// Main class responsible for drawing Inventory's for configuration.
/// </summary>
public static unsafe class InventoryRenderer {
	/// <summary>
	/// Draws an entire inventory.
	/// </summary>
	/// <param name="inventory"></param>
	public static void DrawInventory(InventoryType inventory) {
		using var group = ImRaii.Group();

		var drawOptions = new DrawOptions {
			Scale = 1.33f,
		};

		using var spacing = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, ImGuiHelpers.ScaledVector2(6.0f, 6.0f));
		const int rowSize = 5;

		foreach (var row in Enumerable.Range(0, inventory.ItemsPerPage / rowSize)) {
			foreach (var column in Enumerable.Range(0, rowSize)) {
				DrawInventorySlot(inventory, row * rowSize + column, drawOptions);
				if (column is not rowSize - 1) {
					ImGui.SameLine();
				}
			}
		}
	}

	/// <summary>
	/// Draws an individual inventory slot.
	/// </summary>
	/// <param name="inventory">Inventory to draw.</param>
	/// <param name="slot">Slot index for that inventory.</param>
	/// <param name="options">Parameters to use to draw slot.</param>
	public static void DrawInventorySlot(InventoryType inventory, int slot, DrawOptions options) {
		using var group = ImRaii.Group();

		var inventoryItem = inventory.GetItem(slot);
		if (inventoryItem is null) return;

		const float borderPadding = 2.0f;
		var itemSpacing = ImGui.GetStyle().ItemSpacing.X;
		var regionAvail = (ImGui.GetContentRegionMax().X - itemSpacing * 2.0f - borderPadding * 2.0f) / 5.0f - itemSpacing;

		var iconSize = ImGuiHelpers.ScaledVector2(regionAvail, regionAvail);
		var iconInnerPadding = ImGuiHelpers.ScaledVector2(borderPadding, borderPadding);
		var startPosition = ImGui.GetCursorPos();
		var windowPosition = ImGui.GetWindowPos();

		ImGui.SetCursorPos(startPosition + iconInnerPadding);
		ImGui.Image(Services.TextureProvider.GetFromGameIcon(inventoryItem->IconId).GetWrapOrEmpty().Handle, iconSize);

		var slotSet = GetSettingsForSlot(inventory, slot);

		var borderColor = slotSet?.SetColor ?? options.OutlineColor;
		var hoveredColor = borderColor.Lighten(0.66f);

		var finalOutlineColor = ImGui.IsItemHovered() ? ImGui.GetColorU32(hoveredColor) : ImGui.GetColorU32(borderColor);

		ImGui.GetWindowDrawList().AddRect(
			windowPosition + startPosition,
			windowPosition + startPosition + iconInnerPadding * 2.0f + iconSize,
			finalOutlineColor,
			iconSize.X / 8.0f,
			options.BorderThickness
		);

		if (ImGui.IsItemClicked() && SlotSetConfiguration.EditingSlotSet is { } editingSlotSet && SlotSetConfiguration.EditModeEnabled) {
			if (editingSlotSet.SlotIndexes.Contains(slot)) {
				editingSlotSet.SlotIndexes.Remove(slot);
			}
			else {
				editingSlotSet.SlotIndexes.Add(slot);
			}
		}

		ImGui.SetCursorPos(startPosition + iconSize + iconInnerPadding * 2.0f);
	}

	/// <summary>
	/// Get the slot settings for this slot.
	/// </summary>
	/// <param name="inventory">Inventory containing settings</param>
	/// <param name="slot"></param>
	/// <returns></returns>
	private static SlotSet? GetSettingsForSlot(InventoryType inventory, int slot) {
		if (System.CharacterConfiguration is not {} characterConfiguration) return null;
		if (!characterConfiguration.Inventories.TryGetValue(inventory, out var inventoryConfig)) return null;

		return inventoryConfig.SlotSets.FirstOrDefault(set => set.SlotIndexes.Contains(slot));
	}
}