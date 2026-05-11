using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Classes;
using SortaKinda.Windows.UiParts;

namespace SortaKinda.Windows;

public class ConfigWindow : Window {
	public static InventoryType SelectedInventory = InventoryType.Inventory1;

	private readonly List<InventoryType> allowedInventories = [
		InventoryType.Inventory1,
		InventoryType.Inventory2,
		InventoryType.Inventory3,
		InventoryType.Inventory4,
		InventoryType.ArmoryMainHand,
		InventoryType.ArmoryHead,
		InventoryType.ArmoryBody,
		InventoryType.ArmoryHands,
		InventoryType.ArmoryLegs,
		InventoryType.ArmoryFeets,
		InventoryType.ArmoryOffHand,
		InventoryType.ArmoryEar,
		InventoryType.ArmoryNeck,
		InventoryType.ArmoryWrist,
		InventoryType.ArmoryRings,
		InventoryType.ArmorySoulCrystal,
	];

	public ConfigWindow() : base("SortaKinda Configuration") {
		SizeConstraints = new WindowSizeConstraints {
			MinimumSize = new Vector2(725.0f, 625.0f),
			MaximumSize = new Vector2(725.0f, 625.0f),
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

	private void DrawSlotSetTab() {
		using var slotSetTab = ImRaii.TabItem("Slot Sets");
		if (!slotSetTab) return;

		ImGui.Spacing();

		using var child = ImRaii.Child("TabChild", ImGui.GetContentRegionAvail());
		if (!child) return;

		var itemSpacing = ImGui.GetStyle().ItemSpacing.X / 2.0f;
		var regionX = ImGui.GetContentRegionMax().X;
		var regionY = ImGui.GetContentRegionMax().Y;
		using (var inventoryChild = ImRaii.Child("Inventory", new Vector2(regionX * 3.5f / 10.0f - itemSpacing, regionY))) {
			if (inventoryChild) {
				ImGuiHelpers.ScaledDummy(5.0f);

				ImWidget.DrawSelector(allowedInventories, ref SelectedInventory, ImGui.GetContentRegionAvail().X);
				if (System.SystemConfiguration.LastSelectedInventory != SelectedInventory) {
					System.SystemConfiguration.LastSelectedInventory = SelectedInventory;
					System.SystemConfiguration.Save();

					SlotSetConfiguration.EditingSlotSet = null;
				}

				ImGuiHelpers.ScaledDummy(15.0f);

				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 1.0f * ImGuiHelpers.GlobalScale);
				InventoryRenderer.DrawInventory(SelectedInventory);
			}
		}

		ImGui.SameLine();

		using (var configurationChild = ImRaii.Child("Configuration", new Vector2(regionX * 6.5f / 10.0f - itemSpacing, regionY))) {
			if (configurationChild) {
				SlotSetConfiguration.Draw();
			}
		}
	}

	private void DrawRuleSetTab() {
		using var ruleSetTab = ImRaii.TabItem("Rule Sets");
		if (!ruleSetTab) return;

		using var child = ImRaii.Child("TabChild", ImGui.GetContentRegionAvail());
		if (!child) return;
	}

	private void DrawOptionsTab() {
		using var optionsTab = ImRaii.TabItem("Options");
		if (!optionsTab) return;

		using var child = ImRaii.Child("TabChild", ImGui.GetContentRegionAvail());
		if (!child) return;
	}
}