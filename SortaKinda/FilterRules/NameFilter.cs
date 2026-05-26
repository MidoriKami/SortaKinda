using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class NameFilter : FilteringRuleBase {

	public override string Label
		=> "Name";

	// Note, stored as regex strings.
	public List<string> Names = [];

	private string newRegexString = string.Empty;
	private bool addButtonEnabled;
	private bool refocusInput;

	public override bool HasConfiguration
		=> true;

	public override void DrawConfiguration() {
		DrawTextInputChild();
		DrawOptionsChild();
	}

	private void DrawTextInputChild() {
		var childSize = new Vector2(ImGui.GetContentRegionAvail().X, 22.0f * ImGuiHelpers.GlobalScale);
		using var child = ImRaii.Child("InputString", childSize);
		if (!child) return;

		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 22.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.X);

		if (refocusInput) {
			ImGui.SetKeyboardFocusHere();
			refocusInput = false;
		}

		var textSubmitted = ImGui.InputTextWithHint("##FilterString", "Regex name", ref newRegexString, flags: ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll);

		if (ImGui.IsItemEdited()) {
			try {
				if (newRegexString is "") {
					addButtonEnabled = false;
				}
				else {
					_ = new Regex(newRegexString);
					addButtonEnabled = true;
				}
			}
			catch (ArgumentException) {
				addButtonEnabled = false;
			}
		}

		ImGui.SameLine();
		using (ImRaii.Disabled(!addButtonEnabled)) {
			if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus, ImGui.GetContentRegionAvail() / ImGuiHelpers.GlobalScale) || (textSubmitted && addButtonEnabled)) {
				Names.Add(newRegexString);
				newRegexString = string.Empty;
				System.SystemConfiguration.Save();
			}
		}

		if (textSubmitted) {
			refocusInput = true;
			newRegexString = string.Empty;
		}
	}

	private void DrawOptionsChild() {
		using var child = ImRaii.Child("CurrentOptions", ImGui.GetContentRegionAvail());
		if (!child) return;

		string? toRemove = null;

		foreach (var option in Names) {
			using var id = ImRaii.PushId(option);

			if (ImGuiComponents.IconButton(FontAwesomeIcon.Trash)) {
				toRemove = option;
			}

			ImGui.SameLine();

			ImGui.AlignTextToFramePadding();
			ImGui.Text(option);
		}

		if (toRemove is { } removalString) {
			Names.Remove(removalString);
			System.SystemConfiguration.Save();
		}
	}

	protected override unsafe bool EvaluateItem(InventoryItem* item) {
		var itemName = item->Name;

		return Names.Any(name => Regex.IsMatch(itemName, name));
	}
}