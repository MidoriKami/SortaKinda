using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;

namespace SortaKinda.Windows;

public abstract class SelectionWindowBase<T> : Window where T : class {

	protected abstract List<T> OptionsList { get; }
	private readonly List<T> selectedOptions = [];

	public Action<List<T>>? OnSelectionConfirm { get; set; }

	protected SelectionWindowBase(string windowName) : base(windowName) {
		SizeConstraints = new WindowSizeConstraints {
			MinimumSize = new Vector2(300.0f, 400.0f),
		};

		IsOpen = true;
	}

	protected abstract string GetLabelForOption(T option);

	public override void Draw() {
		DrawSelectionChild();
		DrawFooter();
	}

	private void DrawSelectionChild() {
		var childSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 26.0f - ImGui.GetStyle().ItemSpacing.Y);
		using var child = ImRaii.Child("Selection", childSize);
		if (!child) return;

		foreach (var option in OptionsList) {
			if (ImGui.Selectable(GetLabelForOption(option), selectedOptions.Contains(option))) {
				if (!selectedOptions.Remove(option)) {
					selectedOptions.Add(option);
				}
			}
		}
	}

	private void DrawFooter() {
		var childSize = ImGui.GetContentRegionAvail();
		using var child = ImRaii.Child("Footer", childSize);
		if (!child) return;

		var buttonSize = new Vector2(ImGui.GetContentRegionAvail().X / 2.0f - ImGui.GetStyle().ItemSpacing.X, ImGui.GetContentRegionAvail().Y);
		if (ImGui.Button("Confirm", buttonSize)) {
			OnSelectionConfirm?.Invoke(selectedOptions);
			IsOpen = false;
			OnSelectionConfirm = null;
		}

		ImGui.SameLine();

		if (ImGui.Button("Cancel", buttonSize)) {
			IsOpen = false;
			OnSelectionConfirm = null;
		}
	}

	public override void OnClose() {
		base.OnClose();

		System.WindowSystem.RemoveWindow(this);
	}
}