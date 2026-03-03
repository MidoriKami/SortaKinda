using System;
using System.Numerics;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit.Controllers;
using KamiToolKit.Nodes;

namespace SortaKinda.Addons;

public unsafe class InventoryLargeController : AddonController<AddonInventoryExpansion> {

	private TextButtonNode? sortButton;

	public InventoryLargeController() : base("InventoryLarge") {
		OnAttach += AttachNodes;
		OnDetach += DetachNodes;
	}

	private void AttachNodes(AddonInventoryExpansion* addon) {
		var targetNode = addon->RootNode;
		if (targetNode is null) return;
		
		sortButton = new TextButtonNode {
			String = "Sort",
			Size = new Vector2(100.0f, 28.0f),
			Position = new Vector2(19.0f, 412.0f),
			TextTooltip = "SortaKinda: Sort all Inventories",
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
		sortButton?.Dispose();
	}
}