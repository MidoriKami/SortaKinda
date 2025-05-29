using System;
using System.Numerics;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.ComponentNodes;

namespace SortaKinda.Addons;

public unsafe class InventoryExpansionController : AddonController<AddonInventoryExpansion> {

	private TextButtonNode? sortButton;

	public InventoryExpansionController() : base(Service.PluginInterface, "InventoryExpansion") {
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
			Position = new Vector2(19.0f, 742.0f),
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
		System.NativeController.DetachFromAddon(sortButton, addon, () => {
			sortButton?.Dispose();
			sortButton = null;
		});
	}
}