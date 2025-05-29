using System;
using System.Numerics;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.ComponentNodes;

namespace SortaKinda.Addons;

public unsafe class ArmouryBoardController : AddonController<AddonInventoryExpansion> {

	private TextButtonNode? sortButton;

	public ArmouryBoardController() : base(Service.PluginInterface, "ArmouryBoard") {
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

		var inventoryButton = addon->GetNodeById(123);
		if (inventoryButton is not null) {
			inventoryButton->SetXFloat(141.0f);
		}
		
		sortButton = new TextButtonNode {
			Label = "Sort",
			Size = new Vector2(100.0f, 28.0f),
			Position = new Vector2(19.0f, 566.0f),
			Tooltip = "SortaKinda: Sort all Inventories",
			IsVisible = true,
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
		var inventoryButton = addon->GetNodeById(17);
		if (inventoryButton is not null) {
			inventoryButton->SetXFloat(80.0f);
		}
		
		System.NativeController.DetachFromAddon(sortButton, addon, () => {
			sortButton?.Dispose();
			sortButton = null;
		});
	}
}