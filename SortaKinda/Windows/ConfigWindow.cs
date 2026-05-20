using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Windows.UiParts;

namespace SortaKinda.Windows;

public class ConfigWindow : Window {
	internal static InventoryType SelectedInventory = InventoryType.Inventory1;

	public ConfigWindow() : base("SortaKinda Configuration") {
		SizeConstraints = new WindowSizeConstraints {
			MinimumSize = new Vector2(725.0f, 650.0f),
			MaximumSize = new Vector2(float.MaxValue, 650.0f),
		};

		SelectedInventory = System.SystemConfiguration.LastSelectedInventory;
	}

	public override void Draw() {
		using var tabBar = ImRaii.TabBar("TabBar", ImGuiTabBarFlags.Reorderable);
		if (!tabBar) return;

		DrawSlotSetTab();
        DrawRuleSetTab();
        DrawOptionsTab();
	}

	private static void DrawSlotSetTab() {
		using var slotSetTab = ImRaii.TabItem("Slot Sets");
		if (!slotSetTab) return;

		ImGui.Spacing();

		using var child = ImRaii.Child("TabChild", ImGui.GetContentRegionAvail());
		if (!child) return;

		SlotSetConfiguration.Draw();
	}

	private static void DrawRuleSetTab() {
		using var ruleSetTab = ImRaii.TabItem("Rule Sets");
		if (!ruleSetTab) return;

		ImGui.Spacing();

		using var child = ImRaii.Child("TabChild", ImGui.GetContentRegionAvail());
		if (!child) return;

		RuleSetConfiguration.Draw();
	}

	private static void DrawOptionsTab() {
		using var optionsTab = ImRaii.TabItem("Options");
		if (!optionsTab) return;

		ImGui.Spacing();

		using var child = ImRaii.Child("TabChild", ImGui.GetContentRegionAvail());
		if (!child) return;

		GeneralConfiguration.Draw();
	}
}