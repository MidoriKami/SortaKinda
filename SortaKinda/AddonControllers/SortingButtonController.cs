using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Controllers;
using KamiToolKit.Nodes;
using KamiToolKit.Extensions;

namespace SortaKinda.AddonControllers;

public unsafe class SortingButtonController : IDisposable {
	private readonly MultiAddonController multiAddonController;
	private readonly Dictionary<string, NodeBase> nodeDictionary = [];

	public SortingButtonController() {
		multiAddonController = new MultiAddonController {
			AddonNames = ["ArmouryBoard", "Inventory", "InventoryExpansion", "InventoryLarge"],
			OnSetup = AttachSortButton,
			OnFinalize = DetachSortButton,
		};
		multiAddonController.Enable();
	}

	public void Dispose() {
		multiAddonController.Dispose();
	}

	private void AttachSortButton(AtkUnitBase* addon) {
		switch (addon->NameString) {
			case "ArmouryBoard":
				addon->GetNodeById(123)->Position += new Vector2(61.0f, 0.0f);
				break;

			case "Inventory":
				addon->GetNodeById(17)->Position += new Vector2(16.0f, 0.0f);
				addon->GetNodeById(16)->Position += new Vector2(16.0f, 0.0f);
				break;
		}

		ButtonBase buttonNode = addon->NameString switch {
			"Inventory" => new CircleButtonNode {
				Icon = ButtonIcon.Sort, TextTooltip = "[SortaKinda] Sort all Inventories",
			},
			_ => new TextButtonNode {
				String = "Sort", TextTooltip = "[SortaKinda] Sort all Inventories",
			},
		};

		buttonNode.OnClick = System.SortingController.LaunchSortTask;

		buttonNode.Size = addon->NameString switch {
			"Inventory" => new Vector2(28.0f, 28.0f),
			_ => new Vector2(100.0f, 28.0f),
		};

		buttonNode.Position = addon->NameString switch {
			"ArmouryBoard" => new Vector2(19.0f, 566.0f),
			"Inventory" => new Vector2(19.0f, 414.0f),
			"InventoryExpansion" => new Vector2(19.0f, 742.0f),
			"InventoryLarge" => new Vector2(19.0f, 412.0f),
			_ => Vector2.Zero,
		};

		buttonNode.AttachNode(addon);
		nodeDictionary.Add(addon->NameString, buttonNode);
	}

	private void DetachSortButton(AtkUnitBase* addon) {
		if (nodeDictionary.TryGetValue(addon->NameString, out var node)) {
			nodeDictionary.Remove(addon->NameString);
			node.Dispose();

			switch (addon->NameString) {
				case "ArmouryBoard":
					addon->GetNodeById(123)->Position -= new Vector2(61.0f, 0.0f);
					break;

				case "Inventory":
					addon->GetNodeById(17)->Position -= new Vector2(16.0f, 0.0f);
					addon->GetNodeById(16)->Position -= new Vector2(16.0f, 0.0f);
					break;
			}
		}
	}
}