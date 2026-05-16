using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using SortaKinda.Classes;
using SortaKinda.Configuration;

namespace SortaKinda.Windows.UiParts;

/// <summary>
/// Static class for drawing slot-set configuration ui element.
/// </summary>
public static class SlotSetConfiguration {
	internal static SlotSet? EditingSlotSet;
	internal static bool EditModeEnabled;

	/// <summary>
	/// Draws Inventory and Set Configuration.
	/// </summary>
	public static void Draw() {
		DrawInventoryChild();
		ImGui.SameLine();
		DrawSlotSetConfigurationChild();
	}

	/// <summary>
	/// Draws the interactable inventory view, for selecting and deselecting inventory slots.
	/// </summary>
	private static void DrawInventoryChild() {
		var childSize = new Vector2(250.0f * ImGuiHelpers.GlobalScale, ImGui.GetContentRegionAvail().Y);
		using var child = ImRaii.Child("Inventory", childSize, false, ImGuiWindowFlags.NoMove);
		if (!child) return;

		ImGuiHelpers.ScaledDummy(5.0f);

		ImWidget.DrawSelector(System.AllowedInventories, ref ConfigWindow.SelectedInventory, ImGui.GetContentRegionAvail().X);

		// If selection has changed, we need to save the new selection, but also hide any active set we're editing.
		if (System.SystemConfiguration.LastSelectedInventory != ConfigWindow.SelectedInventory) {
			System.SystemConfiguration.LastSelectedInventory = ConfigWindow.SelectedInventory;
			System.SystemConfiguration.Save();

			EditingSlotSet = null;
		}

		ImGuiHelpers.ScaledDummy(15.0f);

		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 1.0f * ImGuiHelpers.GlobalScale);
		InventoryRenderer.DrawInventory(ConfigWindow.SelectedInventory);
	}

	/// <summary>
	/// Draws everything relating to the configuration of a slot set.
	/// </summary>
	private static void DrawSlotSetConfigurationChild() {
		var childSize = ImGui.GetContentRegionAvail();
		using var child = ImRaii.Child("SlotSetConfiguration", childSize);
		if (!child) return;

		DrawConfigurationHeaderChild();
		DrawConfigurationChild();
	}

	/// <summary>
	/// Draws the header that contains the set selection / set creation
	/// </summary>
	private static void DrawConfigurationHeaderChild() {
		var childSize = new Vector2(ImGui.GetContentRegionAvail().X, 85.0f * ImGuiHelpers.GlobalScale);
		using var child = ImRaii.Child("SlotSetConfigurationHeader", childSize);
		if (!child) return;

		if (System.CharacterConfiguration is not { } config) return;

		// If we don't have a config for this inventory yet, make one
		if (config.Inventories.TryAdd(ConfigWindow.SelectedInventory, new InventoryConfig())) {
			config.Save();
		}

		ImGuiHelpers.CenteredText("Slot Set Configuration");

		ImGui.SameLine();
		ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 24.0f * ImGuiHelpers.GlobalScale);

		using (Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
			ImGui.Text(FontAwesomeIcon.InfoCircle.ToIconString());
		}

		if (ImGui.IsItemHovered()) {
			using (ImRaii.Tooltip())
			using (ImRaii.TextWrapPos(ImGui.GetFontSize() * 36.0f)) {
				ImGui.Text("SortaKinda allows you to define which slots of your inventory will contain which items.\n\n" +
				           "Start by segmenting your inventory into 'Slot Sets' that you can later assign a " +
				           "'Rule Set' to define what items will get sorted to these slots.");
			}
		}

		ImGui.Spacing();
		ImGui.Separator();
		ImGuiHelpers.ScaledDummy(5.0f);

		DrawSlotSelectionTable();

		ImGuiHelpers.ScaledDummy(5.0f);
		ImGui.Separator();
	}

	/// <summary>
	/// Draws a table for the dropdown + create new button.
	/// </summary>
	private static void DrawSlotSelectionTable() {
		using var table = ImRaii.Table("SelectionTable", 2);
		if (!table) return;

		ImGui.TableSetupColumn("DropDown", ImGuiTableColumnFlags.WidthStretch);
		ImGui.TableSetupColumn("AddNew", ImGuiTableColumnFlags.WidthFixed, 26.0f * ImGuiHelpers.GlobalScale);

		ImGui.TableNextColumn();
		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
		DrawSlotSetSelectionCombo();

		ImGui.TableNextColumn();
		if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus)) {
			AddNewSlotSet();
		}
	}

	/// <summary>
	/// Draws the configurable elements, including a delete button.
	/// </summary>
	private static void DrawConfigurationChild() {
		if (System.CharacterConfiguration is not { } config) return;

		using var child = ImRaii.Child("ConfigurationChild", ImGui.GetContentRegionAvail());
		if (!child) return;

		// If we aren't editing a set, don't display edit stuff. Duh.
		if (EditingSlotSet is null) {
			const string warningText = "Select or Create a Slot Set Above";
			var textSize = ImGui.CalcTextSize(warningText);

			ImGui.SetCursorPos(ImGui.GetContentRegionAvail() / 2.0f - textSize / 2.0f);
			ImGui.TextColored(KnownColor.Orange.Vector(), warningText);

			return;
		}

		DrawConfigLabel("Slot Set Name");
		ImGui.InputText("##Set Name", ref EditingSlotSet.Name);
		if (ImGui.IsItemDeactivatedAfterEdit()) {
			config.Save();
		}

		DrawConfigLabel("Edit Mode");
		if (ImGui.Button($"{(EditModeEnabled ? "Enabled" : "Disabled")}##EditMode", new Vector2(ImGui.GetContentRegionAvail().X, 24.0f * ImGuiHelpers.GlobalScale))) {
			EditModeEnabled = !EditModeEnabled;
		}

		if (ImGui.IsItemHovered()) {
			ImGui.SetTooltip("Choose Inventory Slots on the left to add them to this slot set.\n\n" +
			                 "Inventory slots can only belong to one set.\n" +
			                 "If a slot is already assigned to another set, you must first unselect it from that set.");
		}

		DrawConfigLabel("Priority");
		ImGui.InputInt("##Priority", ref EditingSlotSet.Priority);

		if (ImGui.IsItemHovered()) {
			ImGui.SetTooltip("Slot Sets with higher number will be the ones that get the item after a sort.");
		}

		if (ImGui.IsItemDeactivatedAfterEdit()) {
			config.Save();
		}

		DrawConfigLabel("Color");
		ImGui.ColorEdit4("##Color", ref EditingSlotSet.SetColor);
		if (ImGui.IsItemDeactivatedAfterEdit()) {
			config.Save();
		}

		DrawConfigLabel("Rule Set");
		DrawRuleSetSelectCombo();

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

	/// <summary>
	/// Draws a combo for selecting already defined rulesets, or a notice if none are defined.
	/// </summary>
	private static void DrawRuleSetSelectCombo() {
		if (System.CharacterConfiguration is not { } config) return;
		if (EditingSlotSet is null) return;

		var ruleSet = System.SystemConfiguration.RuleSets.FirstOrDefault(ruleset => ruleset.RuleSetId == EditingSlotSet.RuleSetId);

		using var combo = ImRaii.Combo("##RuleSetSelect", ruleSet?.Name ?? "Select a Rule Set");
		if (!combo) return;

		var ruleSets = System.SystemConfiguration.RuleSets;

		if (ruleSets.Count is 0) {
			ImGui.Text("None Available - Add one in 'Rule Sets' tab");
			return;
		}

		foreach (var ruleset in ruleSets) {
			if (ImGui.Selectable(ruleset.Name, ruleset.RuleSetId == EditingSlotSet.RuleSetId)) {
				EditingSlotSet.RuleSetId = ruleset.RuleSetId;
				config.Save();
			}
		}
	}

	/// <summary>
	/// Draws a label with a relative amount of padding to the next config element.
	/// </summary>
	/// <param name="label"></param>
	private static void DrawConfigLabel(string label) {
		var labelSize = ImGui.GetContentRegionMax().X * 3.0f / 10.0f;

		ImGuiHelpers.ScaledDummy(5.0f);

		ImGui.AlignTextToFramePadding();
		ImGui.Text(label);
		ImGui.SameLine(labelSize);
		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
	}

	/// <summary>
	/// Draws combo box with all available Slot Sets, or shows "Add New SlotSet" that will create a new set.
	/// </summary>
	private static void DrawSlotSetSelectionCombo() {
		if (System.CharacterConfiguration is not { } config) return;

		var inventoryConfig = config.Inventories[ConfigWindow.SelectedInventory];
		var slotSets = inventoryConfig.SlotSets;

		using var combo = ImRaii.Combo("##SlotSetsCombo", EditingSlotSet?.Name ?? "Select a Slot Set to Edit", ImGuiComboFlags.HeightLarge);
		if (!combo) return;

		if (slotSets.Count is 0) {
			if (ImGui.Selectable("Add New SlotSet")) {
				AddNewSlotSet();
			}
			return;
		}

		foreach (var (index, slotSet) in slotSets.Index()) {
			using var id = ImRaii.PushId(index);

			var cursorPosition = ImGui.GetCursorPos();
			if (ImGui.Selectable("##Name", EditingSlotSet == slotSet)) {
				EditingSlotSet = slotSet;
				EditModeEnabled = false;
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

	/// <summary>
	/// Adds a new slot set, clears editing set, and saves new set.
	/// </summary>
	private static void AddNewSlotSet() {
		if (System.CharacterConfiguration is not { } config) return;
		var inventoryConfig = config.Inventories[ConfigWindow.SelectedInventory];

		var adjustedHue = 0.07f * inventoryConfig.SlotSets.Count;
		var hsvaColor = new ColorHelpers.HsvaColor(adjustedHue, 1.0f, 1.0f, 1.0f);

		var newSlotSet = new SlotSet {
			SetColor = ColorHelpers.HsvToRgb(hsvaColor),
		};

		inventoryConfig.SlotSets.Add(newSlotSet);
		EditingSlotSet = newSlotSet;

		config.Save();

		EditModeEnabled = false;
	}
}