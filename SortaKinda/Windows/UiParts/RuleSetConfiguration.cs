using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using SortaKinda.Configuration;
using SortaKinda.Extensions;

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
		using var child = ImRaii.Child("Inventory", childSize);
		if (!child) return;

		if (System.SystemConfiguration is not { } config) return;

		DrawRuleSetListBox();

		ImGui.SetCursorPosY(ImGui.GetContentRegionMax().Y - 24.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.Y);
		if (ImGui.Button("Add Rule Set", ImGui.GetContentRegionAvail())) {
			var newRuleSet = new RuleSet();
			config.RuleSets.Add(newRuleSet);
			selectedRuleSet = newRuleSet;
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

		foreach (var (index, entry) in config.RuleSets.Index()) {
			if (ImGui.Selectable($"{entry.Name}##{index}", entry == selectedRuleSet)) {
				if (entry == selectedRuleSet) {
					selectedRuleSet = null;
				}
				else {
					selectedRuleSet = entry;
				}
			}
		}
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
		ImGuiHelpers.ScaledDummy(5.0f);

		if (selectedRuleSet is null) {
			const string warningText = "Select or Create a Rule Set on the left";
			var textSize = ImGui.CalcTextSize(warningText);

			ImGui.SetCursorPos(ImGui.GetContentRegionAvail() / 2.0f - textSize / 2.0f);
			ImGui.TextColored(KnownColor.Orange.Vector(), warningText);
		}
	}
}