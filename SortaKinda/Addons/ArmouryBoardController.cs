using System;
using System.Numerics;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit.Classes.Controllers;
using KamiToolKit.Nodes;

namespace SortaKinda.Addons;

public unsafe class ArmouryBoardController : AddonController<AddonInventoryExpansion> {

	private TextButtonNode? sortButton;

	public ArmouryBoardController() : base("ArmouryBoard") {
		OnAttach += AttachNodes;
		OnDetach += DetachNodes;
	}

	private void AttachNodes(AddonInventoryExpansion* addon) {
		var targetNode = addon->RootNode;
		if (targetNode is null) return;

		var inventoryButton = addon->GetNodeById(123);
		if (inventoryButton is not null) {
			inventoryButton->SetXFloat(141.0f);
		}
		
		sortButton = new TextButtonNode {
			String = "Sort",
			Size = new Vector2(100.0f, 28.0f),
			Position = new Vector2(19.0f, 566.0f),
			Tooltip = "SortaKinda: Sort all Inventories",
			IsVisible = true,
		};
		
		sortButton.OnClick = () => {
			System.ModuleController.Sort();
				
			sortButton.HideTooltip();
			sortButton.IsEnabled = false;
			Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => sortButton.IsEnabled = true);
		};
		sortButton.AttachNode(targetNode);
	}

	private void DetachNodes(AddonInventoryExpansion* addon) {
		var inventoryButton = addon->GetNodeById(17);
		if (inventoryButton is not null) {
			inventoryButton->SetXFloat(80.0f);
		}

		sortButton?.Dispose();
	}
}