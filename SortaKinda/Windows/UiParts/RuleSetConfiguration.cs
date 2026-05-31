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
using SortaKinda.FilterRules;
using SortaKinda.OrderRules;

namespace SortaKinda.Windows.UiParts;

public static class RuleSetConfiguration {
	private static RuleSet? selectedRuleSet;

	public static void Draw() {
		DrawRuleSetListChild();
		ImGui.SameLine();
		DrawRuleSetConfigurationChild();
	}

	private static void DrawRuleSetListChild() {
		var childSize = new Vector2(250.0f * ImGuiHelpers.GlobalScale, ImGui.GetContentRegionAvail().Y);
		using var child = ImRaii.Child("ListBox", childSize);
		if (!child) return;

		if (System.SystemConfiguration is not { } config) return;

		DrawRuleSetListBox();

		var addButtonSize = new Vector2(ImGui.GetContentRegionAvail().X * 5.0f / 10.0f - ImGui.GetStyle().ItemSpacing.X * 2.0f / 3.0f, ImGui.GetContentRegionAvail().Y);
		var importButtonSize = new Vector2(ImGui.GetContentRegionAvail().X * 2.5f / 10.0f - ImGui.GetStyle().ItemSpacing.X * 2.0f / 3.0f, ImGui.GetContentRegionAvail().Y);

		ImGui.SetCursorPosY(ImGui.GetContentRegionMax().Y - 22.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.Y);
		if (ImGui.Button("Add Rule Set", addButtonSize)) {
			var adjustedHue = 0.07f * config.RuleSets.Count;
			var hsvaColor = new ColorHelpers.HsvaColor(adjustedHue, 1.0f, 1.0f, 1.0f);

			config.RuleSets.Add(selectedRuleSet = new RuleSet {
				Color = ColorHelpers.HsvToRgb(hsvaColor),
			});
			config.Save();
		}

		ImGui.SameLine();
		if (ImGuiComponents.IconButton(FontAwesomeIcon.FileImport, importButtonSize / ImGuiHelpers.GlobalScale)) {
			PresetManager.LoadFromClipboard();
		}

		if (ImGui.IsItemHovered()) {
			ImGui.SetTooltip("Import Ruleset(s) from Clipboard");
		}

		ImGui.SameLine();
		if (ImGuiComponents.IconButton(FontAwesomeIcon.FileExport, importButtonSize / ImGuiHelpers.GlobalScale)) {
			PresetManager.SaveToClipboard();
		}

		if (ImGui.IsItemHovered()) {
			ImGui.SetTooltip("Export Ruleset(s) to Clipboard");
		}
	}

	private static void DrawRuleSetConfigurationChild() {
		var childSize = ImGui.GetContentRegionAvail();
		using var child = ImRaii.Child("SlotSetConfiguration", childSize);
		if (!child) return;

		if (System.SystemConfiguration is not { } config) return;

		DrawRuleSetConfiguration();

		if (selectedRuleSet is null) return;

		ImGui.SetCursorPosY(ImGui.GetContentRegionMax().Y - 24.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.Y);
		using (ImRaii.Disabled(!Services.KeyState.DeleteKeybindPressed)) {
			if (ImGui.Button("Delete", ImGui.GetContentRegionAvail())) {
				config.RuleSets.Remove(selectedRuleSet);
				selectedRuleSet = null;

				SlotSetConfiguration.EditingSlotSet = null;
				SlotSetConfiguration.EditModeEnabled = false;

				if (System.CharacterConfiguration?.PurgeInvalidSlotSets() ?? false) {
					System.CharacterConfiguration.Save();
				}

				config.Save();
			}
		}

		if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && !Services.KeyState.DeleteKeybindPressed) {
			ImGui.SetTooltip("Hold Control + Shift to enable button.");
		}
	}

	private static void DrawRuleSetListBox() {
		if (System.SystemConfiguration is not { } config) return;

		var footerHeight = 26.0f * ImGuiHelpers.GlobalScale + ImGui.GetStyle().ItemSpacing.Y;
		using var list = ImRaii.ListBox("RuleSetList", new Vector2(ImGui.GetContentRegionMax().X, ImGui.GetContentRegionMax().Y - footerHeight));
		if (!list) return;

		var ruleSets = config.RuleSets
			.Where(ruleSet => ruleSet.RuleSetId != SlotSet.IgnoreSlotsId)
			.OrderBy(entry => entry.Name);

		foreach (var (index, entry) in ruleSets.Index()) {
			using var id = ImRaii.PushId(index);

			if (ImWidget.DrawColoredSelectable(entry.Color, entry.Name, entry == selectedRuleSet)) {
				if (entry == selectedRuleSet) {
					selectedRuleSet = null;
				}
				else {
					selectedRuleSet = entry;
				}
			}
		}
	}

	/// <summary>
	/// Draws a label with a relative amount of padding to the next config element.
	/// </summary>
	/// <param name="label"></param>
	private static void DrawConfigLabel(string label) {
		ImGuiHelpers.ScaledDummy(5.0f);

		ImGui.AlignTextToFramePadding();
		ImGui.Text(label);
		ImGui.SameLine(ImGui.GetContentRegionMax().X * 3.0f / 10.0f);
		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
	}

	private static void DrawRuleSetConfiguration() {
		if (System.SystemConfiguration is not { } config) return;

		ImGuiHelpers.CenteredText("Rule Set Configuration");

		ImGui.SameLine();
		ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 24.0f * ImGuiHelpers.GlobalScale);

		using (Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
			ImGui.Text(FontAwesomeIcon.InfoCircle.ToIconString());
		}

		if (ImGui.IsItemHovered()) {
			using (ImRaii.Tooltip())
			using (ImRaii.TextWrapPos(ImGui.GetFontSize() * 36.0f)) {
				ImGui.Text("SortaKinda allows you to define 'Rule Sets' which will define what items are allowed/disallowed and in what order.");
			}
		}

		ImGui.Spacing();
		ImGui.Separator();

		if (selectedRuleSet is null) {
			ImWidget.TextCenteredMultiline(KnownColor.Orange.Vector(),
				"Select or Create a Rule Set on the left"
			);
			return;
		}

		DrawConfigLabel("Rule Set Name");
		ImGui.InputText("##Name", ref selectedRuleSet.Name, flags: ImGuiInputTextFlags.AutoSelectAll);

		DrawConfigLabel("Color");
		ImGui.ColorEdit4("#Color",  ref selectedRuleSet.Color);

		if (ImGui.IsItemDeactivatedAfterEdit()) {
			config.Save();
		}

		DrawConfigLabel("Filter Mode");
		var cursorPositon = ImGui.GetCursorPos();
		var halfWidth = ImGui.GetContentRegionAvail().X / 2.0f - ImGui.GetStyle().ItemSpacing.X / 2.0f;

		if (ImGui.RadioButton("Matches All", selectedRuleSet.RequireAll)) {
			selectedRuleSet.RequireAll = true;
			config.Save();
		}

		ImGui.SameLine(cursorPositon.X + halfWidth);

		if (ImGui.RadioButton("Matches Any", !selectedRuleSet.RequireAll)) {
			selectedRuleSet.RequireAll = false;
			config.Save();
		}

		if (ImGui.IsItemDeactivatedAfterEdit()) {
			config.Save();
		}

		ImGuiHelpers.ScaledDummy(10.0f);

		DrawFilterConfigChild();
		DrawOrderingConfigChild();
	}

	private static void DrawFilterConfigChild() {
		if (selectedRuleSet is null) return;

		ImGui.Text("Filter Rules");
		ImGui.SameLine();
		ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 24.0f * ImGuiHelpers.GlobalScale);

		using (Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
			ImGui.Text(FontAwesomeIcon.InfoCircle.ToIconString());
		}

		if (ImGui.IsItemHovered()) {
			using (ImRaii.Tooltip())
			using (ImRaii.TextWrapPos(ImGui.GetFontSize() * 24.0f)) {
				ImGui.Text("Rules that define what items will be allowed or disallowed from filling a particular Rule Set's slots.");
			}
		}

		ImGui.Spacing();
		ImGui.Separator();

		if (ImGui.Button("Add Filter Rule", new Vector2(ImGui.GetContentRegionAvail().X, 24.0f * ImGuiHelpers.GlobalScale))) {
			if (System.WindowSystem.Windows.FirstOrDefault(window => window is FilterSelectWindow) is {} filterSelectWindow) {
				filterSelectWindow.Collapsed = false;
				filterSelectWindow.IsOpen = true;
			}
			else {
				System.WindowSystem.AddWindow(new FilterSelectWindow {
					OptionsList = System.GetFilteringRules(),
					OnSelectionConfirm = selectedOptions => {
						selectedOptions.RemoveAll(option
							=> selectedRuleSet.FilterRules.Any(existingRule
								=> existingRule.GetType() == option.GetType()));

						selectedRuleSet.FilterRules.AddRange(selectedOptions);
						System.SystemConfiguration.Save();
					},
				});
			}
		}

		var childSize = new Vector2(ImGui.GetContentRegionMax().X, ImGui.GetContentRegionAvail().Y * 3.55f / 10.0f);
		using var child = ImRaii.Child("FilterConfig", childSize);
		if (!child) return;

		if (selectedRuleSet.FilterRules.Count is 0) {
			ImWidget.TextCenteredMultiline(KnownColor.Orange.Vector(),
				"No Filtering Rules Defined",
				"At Least One Filter is Required"
			);
			return;
		}

		ImGui.Spacing();

		DrawFilterRuleTable();
	}

	/// <summary>
	/// Draws a table with all the enabled filter rules for this ruleset, including configuration buttons.
	/// </summary>
	private static void DrawFilterRuleTable() {
		if (selectedRuleSet is null) return;

		using var table = ImRaii.Table("FiltersTable", 4);
		if (!table) return;

		ImGui.TableSetupColumn("AllowedState", ImGuiTableColumnFlags.WidthFixed, 75.0f * ImGuiHelpers.GlobalScale);
		ImGui.TableSetupColumn("Label",  ImGuiTableColumnFlags.WidthStretch);
		ImGui.TableSetupColumn("Configure", ImGuiTableColumnFlags.WidthFixed, 75.0f * ImGuiHelpers.GlobalScale);
		ImGui.TableSetupColumn("Delete",  ImGuiTableColumnFlags.WidthFixed, 75.0f * ImGuiHelpers.GlobalScale);

		FilteringRuleBase? filterToRemove = null;

		foreach (var (index, filter) in selectedRuleSet.FilterRules.Index()) {
			using var id = ImRaii.PushId($"{selectedRuleSet.RuleSetId.ToString()}{index}");

			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.AlignTextToFramePadding();

			using (ImRaii.PushColor(ImGuiCol.Text, filter.IsAllowed ? KnownColor.LimeGreen.Vector() : KnownColor.DarkOrange.Vector())) {
				if (ImGui.Button(filter.IsAllowed ? "Allow" : "Disallow", new Vector2(ImGui.GetContentRegionAvail().X, 22.0f * ImGuiHelpers.GlobalScale))) {
					filter.IsAllowed = !filter.IsAllowed;
					System.SystemConfiguration.Save();
				}
			}

			ImGui.TableNextColumn();
			ImGui.AlignTextToFramePadding();
			ImGui.Text(filter.Label);

			ImGui.TableNextColumn();
			using (ImRaii.Disabled(!filter.HasConfiguration)) {
				if (ImGui.Button("Configure", new Vector2(ImGui.GetContentRegionAvail().X, 22.0f * ImGuiHelpers.GlobalScale))) {
					if (System.WindowSystem.Windows.FirstOrDefault(window => window is ConfigurationPopupWindow) is ConfigurationPopupWindow popupConfigWindow) {
						popupConfigWindow.DrawAction = filter.DrawConfiguration;
						popupConfigWindow.WindowName = $"{filter.Label} Filter Configuration";
					}
					else {
						System.WindowSystem.AddWindow(new ConfigurationPopupWindow($"{filter.Label} Filter Configuration") {
							DrawAction = filter.DrawConfiguration,
						});
					}
				}
			}

			if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && !filter.HasConfiguration) {
				ImGui.SetTooltip("This rule is not configurable.");
			}

			ImGui.TableNextColumn();
			using (ImRaii.Disabled(!Services.KeyState.DeleteKeybindPressed)) {
				if (ImGui.Button("Delete", new Vector2(ImGui.GetContentRegionAvail().X, 22.0f * ImGuiHelpers.GlobalScale))) {
					filterToRemove = filter;
				}
			}

			if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && !Services.KeyState.DeleteKeybindPressed) {
				ImGui.SetTooltip("Hold Control + Shift to enable button.");
			}
		}

		if (filterToRemove is not null) {
			selectedRuleSet.FilterRules.Remove(filterToRemove);
			System.SystemConfiguration.Save();
		}
	}

	private static void DrawOrderingConfigChild() {
		if (selectedRuleSet is null) return;

		DrawOrderingModeConfig();
		DrawOrderingRulesHeader();

		var footerSpacing = ImGuiHelpers.ScaledVector2(0.0f, 24.0f + ImGui.GetStyle().ItemInnerSpacing.Y * 2.0f);
		var childSize = ImGui.GetContentRegionAvail() - footerSpacing;
		using var child = ImRaii.Child("OrderingConfig", childSize);
		if (!child) return;

		if (selectedRuleSet.OrderingRules.Count is 0) {
			ImWidget.TextCenteredMultiline(KnownColor.Orange.Vector(),
				"No Ordering Rules Defined",
				"Default behavior is alphabetical"
			);
			return;
		}

		ImGui.Spacing();

		DrawOrderingRuleTable();
	}

	private static void DrawOrderingModeConfig() {
		if (selectedRuleSet is null) return;

		ImGui.Text("Ordering Mode");

		ImGui.SameLine();
		ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 24.0f * ImGuiHelpers.GlobalScale);

		using (Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
			ImGui.Text(FontAwesomeIcon.InfoCircle.ToIconString());
		}

		if (ImGui.IsItemHovered()) {
			using (ImRaii.Tooltip())
			using (ImRaii.TextWrapPos(ImGui.GetFontSize() * 36.0f)) {
				ImGui.Text("When Enabled, fills the inventory slots from End to Beginning");
			}
		}

		ImGui.Spacing();
		ImGui.Separator();

		if (ImGui.Checkbox("Reverse Fill", ref selectedRuleSet.ReverseFill)) {
			System.SystemConfiguration.Save();
		}
		ImGuiHelpers.ScaledDummy(5.0f);
	}

	private static void DrawOrderingRulesHeader() {
		if (selectedRuleSet is null) return;

		ImGui.Text("Ordering Rules");
		ImGui.SameLine();
		ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 24.0f * ImGuiHelpers.GlobalScale);

		using (Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
			ImGui.Text(FontAwesomeIcon.InfoCircle.ToIconString());
		}

		if (ImGui.IsItemHovered()) {
			using (ImRaii.Tooltip())
			using (ImRaii.TextWrapPos(ImGui.GetFontSize() * 36.0f)) {
				ImGui.Text("Rules that define what order items will fill a particular Rule Set's slots\nSubsequent rules are only evaluated as tiebreakers");
			}
		}

		ImGui.Spacing();
		ImGui.Separator();

		if (ImGui.Button("Add Ordering Rule", new Vector2(ImGui.GetContentRegionAvail().X, 24.0f * ImGuiHelpers.GlobalScale))) {
			if (System.WindowSystem.Windows.FirstOrDefault(window => window is OrderingSelectWindow) is {} orderingSelectWindow) {
				orderingSelectWindow.Collapsed = false;
				orderingSelectWindow.IsOpen = true;
			}
			else {
				System.WindowSystem.AddWindow(new OrderingSelectWindow {
					OptionsList = System.GetOrderingRules(),
					OnSelectionConfirm = options => {
						options.RemoveAll(option
							=> selectedRuleSet.OrderingRules.Any(existingRule
								=> existingRule.GetType() == option.GetType()));

						selectedRuleSet.OrderingRules.AddRange(options);
						System.SystemConfiguration.Save();
					},
				});
			}
		}
	}

	private static void DrawOrderingRuleTable() {
		if (selectedRuleSet is null) return;

		using var table = ImRaii.Table("OrderingRules", 5);
		if (!table) return;

		ImGui.TableSetupColumn("UpButton", ImGuiTableColumnFlags.WidthFixed, 24.0f * ImGuiHelpers.GlobalScale);
		ImGui.TableSetupColumn("DownButton", ImGuiTableColumnFlags.WidthFixed, 24.0f * ImGuiHelpers.GlobalScale);
		ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthStretch);
		ImGui.TableSetupColumn("ReverseButton", ImGuiTableColumnFlags.WidthFixed, 125.0f * ImGuiHelpers.GlobalScale);
		ImGui.TableSetupColumn("Delete", ImGuiTableColumnFlags.WidthFixed, 75.0f * ImGuiHelpers.GlobalScale);

		OrderingRuleBase? orderingToRemove = null;
		int? upMoveIndex = null;
		int? downMoveIndex = null;

		foreach (var (index, ordering) in selectedRuleSet.OrderingRules.Index()) {
			using var id = ImRaii.PushId($"{ordering.Label}{index}");

			ImGui.TableNextRow();

			ImGui.TableNextColumn();
			using (ImRaii.Disabled(index is 0)) {
				if (ImGuiComponents.IconButton(FontAwesomeIcon.ChevronUp)) {
					upMoveIndex = index;
				}
			}

			ImGui.TableNextColumn();
			using (ImRaii.Disabled(index == selectedRuleSet.OrderingRules.Count - 1)) {
				if (ImGuiComponents.IconButton(FontAwesomeIcon.ChevronDown)) {
					downMoveIndex = index;
				}
			}

			ImGui.TableNextColumn();
			ImGui.AlignTextToFramePadding();
			ImGui.Text(ordering.Label);

			ImGui.TableNextColumn();
			if (ImGui.Button(ordering.ButtonLabel, new Vector2(ImGui.GetContentRegionAvail().X, 22.0f * ImGuiHelpers.GlobalScale))) {
				ordering.IsReversed = !ordering.IsReversed;
				System.SystemConfiguration.Save();
			}

			ImGui.TableNextColumn();
			using (ImRaii.Disabled(!Services.KeyState.DeleteKeybindPressed)) {
				if (ImGui.Button("Delete", new Vector2(ImGui.GetContentRegionAvail().X, 22.0f * ImGuiHelpers.GlobalScale))) {
					orderingToRemove = ordering;
				}
			}

			if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && !Services.KeyState.DeleteKeybindPressed) {
				ImGui.SetTooltip("Hold Control + Shift to enable button.");
			}
		}

		if (upMoveIndex is { } upPosition) {
			ref var orderingRules  = ref selectedRuleSet.OrderingRules;
			(orderingRules[upPosition - 1], orderingRules[upPosition]) = (orderingRules[upPosition], orderingRules[upPosition - 1]);
			System.SystemConfiguration.Save();
		}

		if (downMoveIndex is { } downPosition) {
			ref var orderingRules  = ref selectedRuleSet.OrderingRules;
			(orderingRules[downPosition + 1],  orderingRules[downPosition]) = (orderingRules[downPosition], orderingRules[downPosition + 1]);
			System.SystemConfiguration.Save();
		}

		if (orderingToRemove is not null) {
			selectedRuleSet.OrderingRules.Remove(orderingToRemove);
			System.SystemConfiguration.Save();
		}
	}
}