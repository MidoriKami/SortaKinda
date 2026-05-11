using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using SortaKinda.Configuration;
using SortaKinda.Extensions;

namespace SortaKinda.Windows.UiParts;

/// <summary>
/// Static class for drawing slot-set configuration ui element.
/// </summary>
public static class SlotSetConfiguration {
	public static SlotSet? EditingSlotSet;
	public static bool EditModeEnabled;

	public static void Draw() {
		if (System.CharacterConfiguration is not { } config) return;

		var headerHeight = 160.0f * ImGuiHelpers.GlobalScale;

		var headerSize = new Vector2(ImGui.GetContentRegionAvail().X, headerHeight);
		using (var headerChild = ImRaii.Child("Header", headerSize)) {
			if (headerChild) {

				ImGuiHelpers.CenteredText("Slot Set Configuration");
				ImGui.Separator();
				ImGui.Spacing();

				ImGui.TextWrapped("SortaKinda allows you to define which slots of your inventory will contain which items.\n\n" +
				                  "Start by segmenting your inventory into 'Slot Sets' that you can later assign a " +
				                  "'Rule Set' to define what items will get sorted to these slots.");

				ImGuiHelpers.ScaledDummy(10.0f);

				DrawSlotSetSelection();
			}
		}

		var bodySize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionMax().Y - headerHeight - ImGui.GetStyle().ItemSpacing.Y);
		using (var bodyChild = ImRaii.Child("Body", bodySize)) {
			if (bodyChild) {
				ImGui.Spacing();
				ImGui.Separator();
				ImGuiHelpers.ScaledDummy(10.0f);

				DrawConfigureSlotSet(config);
			}
		}
	}

	private static void DrawSlotSetSelection() {
		if (System.CharacterConfiguration is not { } config) return;

		// If we don't have a config for this inventory yet, make one
		if (config.Inventories.TryAdd(ConfigWindow.SelectedInventory, new InventoryConfig())) {
			config.Save();
		}

		var inventoryConfig = config.Inventories[ConfigWindow.SelectedInventory];
		var slotSets = inventoryConfig.SlotSets;

		using var table = ImRaii.Table("SelectionTable", 2);
		if (!table) return;

		ImGui.TableSetupColumn("DropDown", ImGuiTableColumnFlags.WidthStretch);
		ImGui.TableSetupColumn("AddNew", ImGuiTableColumnFlags.WidthFixed, 26.0f * ImGuiHelpers.GlobalScale);

		ImGui.TableNextColumn();
		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
		using (var combo = ImRaii.Combo("##SlotSetsCombo", EditingSlotSet?.Name ?? "Select a Slot Set to Edit")) {
			if (combo) {

				if (slotSets.Count is 0) {
					if (ImGui.Selectable("Add New SlotSet")) {
						AddNewSlotSet(inventoryConfig);
					}
				}
				else {
					foreach (var (index, slotSet) in slotSets.Index()) {
						using var id = ImRaii.PushId(index);

						var cursorPosition = ImGui.GetCursorPos();
						if (ImGui.Selectable("##Name", EditingSlotSet == slotSet)) {
							EditingSlotSet = slotSet;
						}

						ImGui.SameLine();
						ImGui.SetCursorPos(cursorPosition + new Vector2(5.0f * ImGuiHelpers.GlobalScale, 0.0f));

						using (ImRaii.PushColor(ImGuiCol.Text, slotSet.SetColor))
						using (Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
							ImGui.Text(FontAwesomeIcon.Square.ToIconString());
						}

						ImGui.SameLine();
						ImGui.Text(slotSet.Name);
					}
				}
			}
		}

		ImGui.TableNextColumn();
		if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus)) {
			AddNewSlotSet(inventoryConfig);
		}
	}

	private static void DrawConfigureSlotSet(CharacterConfiguration config) {
		if (EditingSlotSet is null) {
			var warningText = "Select or Create a Slot Set Above";
			var textSize = ImGui.CalcTextSize(warningText);

			ImGui.SetCursorPos(ImGui.GetContentRegionAvail() / 2.0f - textSize / 2.0f);
			ImGui.TextColored(KnownColor.Orange.Vector(), warningText);
		}
		else {
			ImGui.AlignTextToFramePadding();
			ImGui.Text("Slot Set Name");
			ImGui.SameLine(150.0f * ImGuiHelpers.GlobalScale);
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			ImGui.InputText("##Set Name", ref EditingSlotSet.Name);
			if (ImGui.IsItemDeactivatedAfterEdit()) {
				config.Save();
			}

			ImGuiHelpers.ScaledDummy(5.0f);

			ImGui.AlignTextToFramePadding();
			ImGui.Text("Edit Mode");
			ImGui.SameLine(150.0f * ImGuiHelpers.GlobalScale);
			if (ImGui.Button($"{(EditModeEnabled ? "Enabled" : "Disabled")}##EditMode", new Vector2(ImGui.GetContentRegionAvail().X, 24.0f * ImGuiHelpers.GlobalScale))) {
				EditModeEnabled = !EditModeEnabled;
			}

			if (ImGui.IsItemHovered()) {
				ImGui.SetTooltip("Choose Inventory Slots on the left to add them to this slot set.\n\n" +
				                 "Inventory slots can only belong to one set.\n" +
				                 "If a slot is already assigned to another set, you must first unselect it from that set.");
			}

			ImGuiHelpers.ScaledDummy(5.0f);

			ImGui.AlignTextToFramePadding();
			ImGui.Text("Color");
			ImGui.SameLine(150.0f * ImGuiHelpers.GlobalScale);
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			ImGui.ColorEdit4("##Color", ref EditingSlotSet.SetColor);
			if (ImGui.IsItemDeactivatedAfterEdit()) {
				config.Save();
			}

			ImGuiHelpers.ScaledDummy(5.0f);

			ImGui.AlignTextToFramePadding();
			ImGui.Text("Rule Set");
			ImGui.SameLine(150.0f * ImGuiHelpers.GlobalScale);
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			using (var combo = ImRaii.Combo("##RuleSetSelect", EditingSlotSet.Ruleset?.Name ?? "Select a Rule Set")) {
				if (combo) {
					var ruleSets = System.SystemConfiguration.SortingRules;

					if (ruleSets.Count is 0) {
						ImGui.Text("None Available - Add one in 'Rule Sets' tab");
					}
					else {
						foreach (var ruleset in ruleSets) {
							ImGui.Selectable(ruleset.Name, ruleset == EditingSlotSet.Ruleset);
						}
					}
				}
			}

			ImGui.SetCursorPosY(ImGui.GetContentRegionMax().Y - 24.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.Y);
			using (ImRaii.Disabled(!Services.KeyState.DeleteKeybindPressed)) {
				if (ImGui.Button("Delete", new Vector2(ImGui.GetContentRegionAvail().X, 24.0f * ImGuiHelpers.GlobalScale))) {
					config.Inventories[ConfigWindow.SelectedInventory].SlotSets.Remove(EditingSlotSet);
					EditingSlotSet = null;
					config.Save();
				}
			}

			if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && !Services.KeyState.DeleteKeybindPressed) {
				ImGui.SetTooltip("Hold Control + Shift to enable button.");
			}
		}
	}

	private static void AddNewSlotSet(InventoryConfig inventoryConfig) {
		if (System.CharacterConfiguration is not { } config) return;

		var newSlotSet = new SlotSet {
			SetColor = ColorHelpers.HsvToRgb(new ColorHelpers.HsvaColor(0.07f * inventoryConfig.SlotSets.Count, 1.0f, 1.0f, 1.0f)),
		};

	inventoryConfig.SlotSets.Add(newSlotSet);
		EditingSlotSet = newSlotSet;

		config.Save();
	}
}