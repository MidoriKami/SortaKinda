using System;
using System.Numerics;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace SortaKinda.Addons;

public unsafe class InventoryLargeController : AddonController<AddonInventoryExpansion> {

	private TextButtonNode? sortButton;

	public InventoryLargeController() : base("InventoryLarge") {
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
		
		sortButton = new TextButtonNode {
			Label = "Sort",
			Size = new Vector2(100.0f, 28.0f),
			Position = new Vector2(19.0f, 412.0f),
			Tooltip = "SortaKinda: Sort all Inventories",
			IsVisible = true,
		};

		sortButton.OnClick = () => {
			System.ModuleController.Sort();

			sortButton.HideTooltip();
			sortButton.IsEnabled = false;
			Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => sortButton.IsEnabled = true);
		};
		
		System.NativeController.AttachNode(sortButton, targetNode, NodePosition.AsLastChild);
	}

	private void DetachNodes(AddonInventoryExpansion* addon) {
		System.NativeController.DisposeNode(ref sortButton);
	}
}