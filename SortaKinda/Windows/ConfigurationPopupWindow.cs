using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;

namespace SortaKinda.Windows;

public class ConfigurationPopupWindow : Window {
	public required Action? DrawAction {
		get;
		set {
			field = value;
			System.SystemConfiguration.Save();
		}
	}

	public ConfigurationPopupWindow(string windowName) : base(windowName) {
		SizeConstraints = new WindowSizeConstraints {
			MinimumSize = new Vector2(300.0f, 250.0f),
		};

		IsOpen = true;
	}

	public override void Draw() {
		var footerSize = 26.0f * ImGuiHelpers.GlobalScale + ImGui.GetStyle().ItemSpacing.Y;
		var childSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - footerSize);
		using (var child = ImRaii.Child("ConfigChild", childSize)) {
			if (child) {
				DrawAction?.Invoke();
			}
		}

		if (ImGui.Button("Save & Close", ImGui.GetContentRegionAvail())) {
			IsOpen = false;
		}
	}

	public override void OnClose() {
		base.OnClose();

		System.WindowSystem.RemoveWindow(this);
		System.SystemConfiguration.Save();
	}
}