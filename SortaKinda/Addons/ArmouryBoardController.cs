using System.Numerics;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace SortaKinda.Addons;

public unsafe class ArmouryBoardController() : AddonController<AddonInventoryExpansion>(Service.PluginInterface, "ArmouryBoard") {

	private TextButton? sortButton;
	
	protected override void AttachNodes(AddonInventoryExpansion* addon) {
		var targetNode = addon->RootNode;
		if (targetNode is null) return;

		var inventoryButton = addon->GetNodeById(123);
		if (inventoryButton is not null) {
			inventoryButton->SetXFloat(141.0f);
		}
		
		sortButton = new TextButton {
			Label = "Sort",
			Size = new Vector2(100.0f, 28.0f),
			Position = new Vector2(19.0f, 566.0f),
			Tooltip = "SortaKinda: Sort all Inventories",
			IsVisible = true,
		};
		
		sortButton.AddEvent(AddonEventType.MouseClick, System.ModuleController.Sort);
		
		System.NativeController.AttachToAddon(sortButton, addon, targetNode, NodePosition.AsLastChild);
	}

	protected override void DetachNodes(AddonInventoryExpansion* addon) {
		var inventoryButton = addon->GetNodeById(17);
		if (inventoryButton is not null) {
			inventoryButton->SetXFloat(80.0f);
		}
		
		sortButton?.Dispose();
		sortButton = null;
	}
}