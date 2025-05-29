using System;
using System.Numerics;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.ComponentNodes;

namespace SortaKinda.Addons;

public unsafe class InventoryController : AddonController<AddonInventoryExpansion> {

	private CircleButtonNode? sortButton;

	public InventoryController() : base(Service.PluginInterface, "Inventory") {
		OnAttach += AttachNodes;
		OnDetach += DetachNodes;
	}

	public override void Dispose() {
		OnAttach -= AttachNodes;
		OnDetach -= DetachNodes;
		
		sortButton?.Dispose();
		
		base.Dispose();
	}

	private void AttachNodes(AddonInventoryExpansion* addon) {
		var targetNode = addon->RootNode;
		if (targetNode is null) return;

		var armoryButton = addon->GetNodeById(17);
		var saddlebagButton = addon->GetNodeById(16);
		if (armoryButton is not null && saddlebagButton is not null) {
			armoryButton->SetXFloat(49.0f);
			saddlebagButton->SetXFloat(49.0f);
		}
		
		sortButton = new CircleButtonNode {
			Size = new Vector2(28.0f, 28.0f),
			Position = new Vector2(19.0f, 414.0f),
			Tooltip = "SortaKinda: Sort all Inventories",
			IsVisible = true,
			Icon = ButtonIcon.Sort,
			OnClick = () => {
				System.ModuleController.Sort();
				
				sortButton!.HideTooltip();
				sortButton!.Disable();
				Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => sortButton!.Enable());
			},
		};

		System.NativeController.AttachToAddon(sortButton, addon, targetNode, NodePosition.AsLastChild);
	}

	private void DetachNodes(AddonInventoryExpansion* addon) {
		var armoryButton = addon->GetNodeById(17);
		var saddlebagButton = addon->GetNodeById(16);
		if (armoryButton is not null && saddlebagButton is not null) {
			armoryButton->SetXFloat(33.0f);
			saddlebagButton->SetXFloat(33.0f);
		}

		System.NativeController.DetachFromAddon(sortButton, addon, () => {
			sortButton?.Dispose();
			sortButton = null;
		});
	}
}