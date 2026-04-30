using System;

namespace SortaKinda.Addons;

public class AddonControllers : IDisposable {

	private readonly InventoryExpansionController inventoryExpansionController = new();
	private readonly InventoryLargeController inventoryLargeController = new();
	private readonly InventoryController inventoryController = new();
	private readonly ArmouryBoardController armouryBoardController = new();

	public void Dispose() {
		inventoryExpansionController.Dispose();
		inventoryLargeController.Dispose();
		inventoryController.Dispose();
		armouryBoardController.Dispose();
	}
}