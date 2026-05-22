using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Configuration;
using SortaKinda.Windows.UiParts;

namespace SortaKinda.Classes;

/// <summary>
/// Main class responsible for drawing Inventory's for configuration.
/// </summary>
public static unsafe class InventoryRenderer {
	private static PaintingState paintingState = PaintingState.Waiting;
	private static Vector2 clickStart = Vector2.Zero;
	private static Vector2 clickEnd = Vector2.Zero;
	private static readonly List<int> PaintedSlots = [];

	/// <summary>
	/// Draws an entire inventory.
	/// </summary>
	/// <param name="inventory"></param>
	public static void DrawInventory(InventoryType inventory) {
		using var group = ImRaii.Group();

		using var inventoryChild = ImRaii.Child("Inventory", ImGui.GetContentRegionAvail());
		if (!inventoryChild) return;

		var drawOptions = new DrawOptions();

		using var spacing = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, ImGuiHelpers.ScaledVector2(6.0f, 6.0f));
		const int rowSize = 5;

		UpdatePainting();

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
	/// Update the drawn box when dragging in the inventory render area.
	/// </summary>
	private static void UpdatePainting() {
		if (SlotSetConfiguration.EditingSlotSet is not { } slotSet) return;
		if (!SlotSetConfiguration.EditModeEnabled) return;

		switch (paintingState) {
			case PaintingState.Waiting: {
				if (ImGui.IsWindowHovered() && ImGui.IsMouseDragging(ImGuiMouseButton.Left)) {
					clickStart = ImGui.GetMousePos();
					clickEnd = clickStart;
					paintingState = PaintingState.Started;
				}
				break;
			}

			case PaintingState.Started: {
				clickEnd = ImGui.GetMousePos();
				ImGui.GetWindowDrawList().AddRect(
					clickStart,
					clickEnd,
					ImGui.GetColorU32(slotSet.RuleSet.Color),
					4.0f,
					2.0f
				);

				if (ImGui.IsMouseReleased(ImGuiMouseButton.Left)) {
					paintingState = PaintingState.Completed;
				}
				break;
			}

			case PaintingState.Completed: {
				paintingState = PaintingState.Waiting;
				PaintedSlots.Clear();

				System.CharacterConfiguration?.Save();
				break;
			}
		}
	}

	/// <summary>
	/// Draws an individual inventory slot.
	/// </summary>
	/// <param name="inventory">Inventory to draw.</param>
	/// <param name="slot">Slot index for that inventory.</param>
	/// <param name="options">Parameters to use to draw slot.</param>
	private static void DrawInventorySlot(InventoryType inventory, int slot, DrawOptions options) {
		using var group = ImRaii.Group();

		var inventoryItem = inventory.GetItem(slot);
		if (inventoryItem is null) return;

		const float borderPadding = 2.0f;
		var itemSpacing = ImGui.GetStyle().ItemSpacing.X;
		var regionAvail = (ImGui.GetContentRegionMax().X - itemSpacing * 2.0f - borderPadding * 2.0f) / 5.0f - itemSpacing;

		var iconSize = new Vector2(regionAvail, regionAvail);
		var iconInnerPadding = ImGuiHelpers.ScaledVector2(borderPadding, borderPadding);
		var startPosition = ImGui.GetCursorPos();
		var windowPosition = ImGui.GetWindowPos();

		ImGui.SetCursorPos(startPosition + iconInnerPadding);
		ImGui.Image(Services.TextureProvider.GetFromGameIcon(
			inventoryItem->IconId).GetWrapOrEmpty().Handle,
			iconSize,
			Vector2.Zero,
			Vector2.One,
			new Vector4(1.0f, 1.0f, 1.0f, options.IconAlpha)
		);

		var slotSet = GetSettingsForSlot(inventory, slot);
		var outlineColor = GetOutlineColor(options.OutlineColor, slotSet);
		var slotIndex = slotSet?.SlotIndexes.IndexOf(slot);

		DrawTooltip(slotSet);

		ProcessPainting(slot, slotSet);

		ImGui.GetWindowDrawList().AddRect(
			windowPosition + startPosition,
			windowPosition + startPosition + iconInnerPadding * 2.0f + iconSize,
			ImGui.GetColorU32(outlineColor),
			iconSize.X / 8.0f,
			options.BorderThickness
		);

		if (slotIndex is { } index && (SlotSetConfiguration.EditingSlotSet is null || SlotSetConfiguration.EditingSlotSet == slotSet)) {
			var textString = $"{index + 1}";
			var textSize = ImGui.CalcTextSize(textString);

			ImGui.SameLine();
			ImGui.SetCursorPos(startPosition + new Vector2(regionAvail - textSize.X, borderPadding));

			ImWidget.TextOutlined(KnownColor.Black.Vector(), KnownColor.White.Vector(), textString);
		}

		ImGui.SetCursorPos(startPosition + iconSize + iconInnerPadding * 2.0f);
	}

	/// <summary>
	/// Processes the click-drag painting for selecting slots.
	/// </summary>
	/// <param name="slot"></param>
	/// <param name="slotSet"></param>
	private static void ProcessPainting(int slot, SlotSet? slotSet) {
		if (SlotSetConfiguration.EditingSlotSet is not { } editingSlotSet || !SlotSetConfiguration.EditModeEnabled) return;

		switch (paintingState) {
			case PaintingState.Waiting:
				if (ImGui.IsItemClicked()) {
					if (editingSlotSet == slotSet && editingSlotSet.SlotIndexes.Remove(slot)) {
						System.CharacterConfiguration?.Save();
					}
					else if (slotSet is null) {
						editingSlotSet.SlotIndexes.Add(slot);
						System.CharacterConfiguration?.Save();
					}
				}
				break;

			case PaintingState.Started:
				var itemMin = ImGui.GetItemRectMin();
				var itemMax = ImGui.GetItemRectMax();

				// If this is the item we clicked on, skip it because it's already changed due to the Waiting case.
				if (itemMin.X <= clickStart.X && itemMax.X >= clickStart.X &&
				    itemMin.Y <= clickStart.Y && itemMax.Y >= clickStart.Y) return;

				var mouseMin = new Vector2(Math.Min(clickStart.X, clickEnd.X), Math.Min(clickStart.Y, clickEnd.Y));
				var mouseMax = new Vector2(Math.Max(clickStart.X, clickEnd.X), Math.Max(clickStart.Y, clickEnd.Y));

				var isIntersecting = itemMin.X <= mouseMax.X && itemMax.X >= mouseMin.X &&
				                     itemMin.Y <= mouseMax.Y && itemMax.Y >= mouseMin.Y;

				if (isIntersecting && !PaintedSlots.Contains(slot)) {
					if (slotSet is null) {
						editingSlotSet.SlotIndexes.Add(slot);
					}
					else if (slotSet == editingSlotSet) {
						editingSlotSet.SlotIndexes.Remove(slot);
					}

					// Track this so we don't undo it by mistake.
					PaintedSlots.Add(slot);
				}
				break;
		}
	}

	/// <summary>
	/// Draws a formatted tooltip to give a preview of what slot set is being applied to this slot.
	/// </summary>
	/// <param name="slotSet"></param>
	private static void DrawTooltip(SlotSet? slotSet) {
		if (!ImGui.IsItemHovered() || slotSet is null) return;

		using var tooltipDisposable = ImRaii.Tooltip();
		using var itemSpacing = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(2.0f, 1.0f));

		using (ImRaii.PushColor(ImGuiCol.Text, slotSet.RuleSet.Color))
		using (Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
			ImGui.Text(FontAwesomeIcon.Square.ToIconString());
		}

		ImGui.SameLine();
		ImGui.Text(slotSet.RuleSet.Name);

		if (slotSet.RuleSet.FilterRules.Count is not 0) {
			ImGui.Text("\nFilters:");

			foreach (var filterRule in slotSet.RuleSet.FilterRules) {
				if (filterRule.IsAllowed) {
					ImGui.TextColored(KnownColor.Green.Vector(), "\tAllow");
				}
				else {
					ImGui.TextColored(KnownColor.Orange.Vector(), "\tDisallow");
				}

				ImGui.SameLine();

				ImGui.Text($"• {filterRule.Label}");
			}
		}

		if (slotSet.RuleSet.OrderingRules.Count is not 0) {
			ImGui.Text("\nOrdering:");

			foreach (var orderingRule in slotSet.RuleSet.OrderingRules) {
				ImGui.Text($"\t{orderingRule.Label}");
			}
		}
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

	private static Vector4 GetOutlineColor(Vector4 outlineColor, SlotSet? slotSet) {
		if (ImGui.IsItemHovered()) {

			// If we have a set selected, and any slot is hovered, use the editing sets color.
			if (SlotSetConfiguration.EditingSlotSet is { } editingSet) {
				outlineColor = editingSet.RuleSet.Color;
			}

			// Else we hovered a slot, with no editing set selected
			else {
				outlineColor = KnownColor.White.Vector();
			}
		}
		else {

			// If we aren't editing a slot set, show the full set color
			if (SlotSetConfiguration.EditingSlotSet is not {} editingSet) {
				if (slotSet is not null) {
					outlineColor = slotSet.RuleSet.Color;
				}
			}

			// We are editing a slot set,
			else {

				// This set isn't the one we are editing
				if (slotSet is not null && slotSet != editingSet) {
					outlineColor = slotSet.RuleSet.Color.Fade(0.66f);
				}

				// This is the set we are editing
				else if (slotSet is not null && slotSet == editingSet) {
					outlineColor = slotSet.RuleSet.Color.Fade(0.20f);
				}
			}
		}

		return outlineColor;
	}
}