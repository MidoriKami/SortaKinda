using System.Numerics;
using Dalamud.Interface.Windowing;

namespace SortaKinda.Windows;

public class ConfigurationPopupWindow : Window {

	public ConfigurationPopupWindow(string windowName) : base(windowName) {
		SizeConstraints = new WindowSizeConstraints {
			MinimumSize = new Vector2(300.0f, 100.0f),
		};

		IsOpen = true;

		System.WindowSystem.AddWindow(this);
	}

	public override void Draw() {

	}

	public override void OnClose() {
		base.OnClose();

		System.WindowSystem.RemoveWindow(this);
	}
}