using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.Classes;

/// <summary>
/// Custom widgets and helpers for drawing more advanced ui elements.
/// </summary>
public static class ImWidget {

	/// <summary>
	/// Draws multiple stings centered in the current content area.
	/// </summary>
	/// <param name="color">Text Color</param>
	/// <param name="strings">Strings, must be 1 or more.</param>
	public static void TextCenteredMultiline(Vector4 color, params string[] strings) {
		if (strings.Length is 0) return;

		var stringHeight = ImGui.CalcTextSize(strings.First()).Y;
		var totalHeight = (stringHeight + ImGui.GetStyle().ItemSpacing.Y) * strings.Length;

		ImGui.SetCursorPosY(ImGui.GetContentRegionAvail().Y / 2.0f - totalHeight / 2.0f);
		foreach (var text in strings) {
			var stringWidth = ImGui.CalcTextSize(text).X;

			ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X / 2.0f - stringWidth / 2.0f);
			ImGui.TextColored(color, text);
		}
	}

	/// <summary>
	/// Draws text 9 times to create an outlined effect.
	/// </summary>
	/// <param name="outlineColor">The outline color</param>
	/// <param name="textColor">The text color</param>
	/// <param name="text">Text to draw</param>
	public static void TextOutlined(Vector4 outlineColor, Vector4 textColor, string text) {
		var startPos = ImGui.GetCursorPos();

		foreach(var x in Enumerable.Range(-1, 3))
		foreach (var y in Enumerable.Range(-1, 3)) {
			if (x is 0 && y is 0) continue;

			ImGui.SetCursorPos(startPos + new Vector2(x, y));
			ImGui.TextColored(outlineColor, text);
		}

		ImGui.SetCursorPos(startPos);
		ImGui.TextColored(textColor, text);
	}

	/// <summary>
	/// Draws a colored box, followed by the text, and works like a ImGui selectable.
	/// </summary>
	/// <param name="color">Color of the box</param>
	/// <param name="text">Label</param>
	/// <param name="isSelected">If this should be drawn highlighted</param>
	/// <returns></returns>
	public static bool DrawColoredSelectable(Vector4 color, string text, bool isSelected = false) {
		var cursorPosition = ImGui.GetCursorPos();
		if (ImGui.Selectable($"##{text}", isSelected)) {
			return true;
		}

		ImGui.SameLine();
		ImGui.SetCursorPos(cursorPosition + new Vector2(5.0f * ImGuiHelpers.GlobalScale, 0.0f));

		using (ImRaii.PushColor(ImGuiCol.Text, color))
		using (SortaKinda.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
			ImGui.Text(FontAwesomeIcon.Square.ToIconString());
		}

		ImGui.SameLine();
		ImGui.Text(text);

		return false;
	}

	/// <summary>
	/// Draws a dropdown combo with centered text, and left/right arrows to increment and decrement values.
	/// </summary>
	/// <param name="values">Values to display</param>
	/// <param name="currentValue">Value for the current label</param>
	/// <param name="width">How large to make center section</param>
	public static void DrawSelector(List<InventoryType> values, ref InventoryType currentValue, float width) {
		using (SortaKinda.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
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

		using (SortaKinda.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {
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