using System;
using System.Numerics;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit.Controllers;
using KamiToolKit.Nodes;
using KamiToolKit.Extensions;

namespace SortaKinda.AddonControllers;

public unsafe class ArmouryBoardController : IDisposable {
	private TextButtonNode? sortButton;
	private readonly AddonController<AddonInventoryExpansion> addonController;

	public ArmouryBoardController() {
		addonController = new AddonController<AddonInventoryExpansion> {
			AddonName = "ArmouryBoard",
			OnSetup = AttachNodes,
			OnFinalize = DetachNodes,
		};

		addonController.Enable();
	}

	public void Dispose()
		=> addonController.Dispose();

	private void AttachNodes(AddonInventoryExpansion* addon) {
		var targetNode = addon->RootNode;
		if (targetNode is null) return;

		var inventoryButton = addon->GetNodeById(123);
		if (inventoryButton is not null) {
			inventoryButton->Position += new Vector2(61.0f, 0.0f);
		}

		sortButton = new TextButtonNode {
			String = "Sort",
			Size = new Vector2(100.0f, 28.0f),
			Position = new Vector2(19.0f, 566.0f),
			TextTooltip = "[SortaKinda] Sort all Inventories",
		};

		sortButton.OnClick = () => {
			System.SortingController.LaunchSortTask();

			sortButton.HideTooltip();
			sortButton.IsEnabled = false;
			Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => sortButton.IsEnabled = true);
		};
		sortButton.AttachNode(targetNode);
	}

	private void DetachNodes(AddonInventoryExpansion* addon) {
		var inventoryButton = addon->GetNodeById(123);
		if (inventoryButton is not null) {
			inventoryButton->Position -= new Vector2(61.0f, 0.0f);
		}

		sortButton?.Dispose();
	}
}