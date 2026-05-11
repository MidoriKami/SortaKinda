using System.Collections.Generic;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Extensions;

namespace SortaKinda.Classes;

/// <summary>
/// Custom widgets and helpers for drawing more advanced ui elements.
/// </summary>
public static class ImWidget {
	public static void DrawSelector(List<InventoryType> values, ref InventoryType currentValue, float width) {
		using (Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
			if (ImGui.Button(FontAwesomeIcon.CaretLeft.ToIconString())) {
				var currentIndex = values.IndexOf(currentValue);
				if (currentIndex is 0) {
					currentValue = values[^1];
				}
				else {
					currentValue = values[currentIndex - 1];
				}
			}
		}

		var buttonSize = ImGui.GetItemRectSize();
		width = width - buttonSize.X * 2.0f - ImGui.GetStyle().ItemSpacing.X * 2.0f;

		ImGui.SameLine();

		var startPos = ImGui.GetCursorPos();
		ImGui.SetNextItemWidth(width);
		using (var combo = ImRaii.Combo($"##{currentValue}Combo", string.Empty, ImGuiComboFlags.HeightLargest | ImGuiComboFlags.NoArrowButton)) {
			if (combo) {
				foreach (var value in values) {
					if (ImGui.Selectable(value.Name, value.Equals(currentValue))) {
						currentValue = value;
					}
				}
			}
		}

		ImGui.SameLine();

		var restorePosition = ImGui.GetCursorPos();
		var textString = currentValue.Name;
		var textSize = ImGui.CalcTextSize(textString);
		ImGui.SetCursorPosX(startPos.X + width / 2.0f - textSize.X / 2.0f);
		ImGui.Text(textString);

		ImGui.SameLine();
		ImGui.SetCursorPos(restorePosition);

		using (Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
			if (ImGui.Button(FontAwesomeIcon.CaretRight.ToIconString())) {
				var currentIndex = values.IndexOf(currentValue);
				if (currentIndex == values.Count - 1) {
					currentValue = values[0];
				}
				else {
					currentValue = values[currentIndex + 1];
				}
			}
		}
	}
}