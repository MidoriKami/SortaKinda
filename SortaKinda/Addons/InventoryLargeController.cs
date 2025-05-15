using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace SortaKinda.Addons;

public unsafe class InventoryLargeController() : AddonController<AddonInventoryExpansion>(Service.PluginInterface, "InventoryLarge") {

	private TextButton? sortButton;
	
	protected override void AttachNodes(AddonInventoryExpansion* addon) {
		var targetNode = addon->RootNode;
		if (targetNode is null) return;
		
		sortButton = new TextButton {
			Label = "Sort",
			Size = new Vector2(100.0f, 28.0f),
			Position = new Vector2(19.0f, 412.0f),
			Tooltip = "SortaKinda: Sort all Inventories",
			IsVisible = true,
		};
		
		sortButton.AddEvent(AddonEventType.MouseClick, System.ModuleController.Sort);
		
		System.NativeController.AttachToAddon(sortButton, addon, targetNode, NodePosition.AsLastChild);
	}

	protected override void DetachNodes(AddonInventoryExpansion* addon) {
		sortButton?.Dispose();
		sortButton = null;
	}
}