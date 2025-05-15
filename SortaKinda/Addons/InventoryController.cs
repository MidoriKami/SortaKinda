using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace SortaKinda.Addons;

public unsafe class InventoryController() : AddonController<AddonInventoryExpansion>(Service.PluginInterface, "Inventory") {

	private CircleButton? sortButton;
	
	protected override void AttachNodes(AddonInventoryExpansion* addon) {
		var targetNode = addon->RootNode;
		if (targetNode is null) return;

		var armoryButton = addon->GetNodeById(17);
		var saddlebagButton = addon->GetNodeById(16);
		if (armoryButton is not null && saddlebagButton is not null) {
			armoryButton->SetXFloat(49.0f);
			saddlebagButton->SetXFloat(49.0f);
		}
		
		sortButton = new CircleButton {
			Size = new Vector2(28.0f, 28.0f),
			Position = new Vector2(19.0f, 414.0f),
			Tooltip = "SortaKinda: Sort all Inventories",
			IsVisible = true,
			Icon = ButtonIcon.Sort,
		};

		sortButton.AddEvent(AddonEventType.MouseClick, System.ModuleController.Sort);

		System.NativeController.AttachToAddon(sortButton, addon, targetNode, NodePosition.AsLastChild);
	}

	protected override void DetachNodes(AddonInventoryExpansion* addon) {
		var armoryButton = addon->GetNodeById(17);
		var saddlebagButton = addon->GetNodeById(16);
		if (armoryButton is not null && saddlebagButton is not null) {
			armoryButton->SetXFloat(33.0f);
			saddlebagButton->SetXFloat(33.0f);
		}

		sortButton?.Dispose();
		sortButton = null;
	}
}