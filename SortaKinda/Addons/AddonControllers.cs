using System;

namespace SortaKinda.Addons;

public class AddonControllers : IDisposable {

	private readonly InventoryExpansionController inventoryExpansionController;
	private readonly InventoryLargeController inventoryLargeController;
	private readonly InventoryController inventoryController;
	private readonly ArmouryBoardController armouryBoardController;
	
	public AddonControllers() {
		inventoryExpansionController = new InventoryExpansionController();
		inventoryLargeController = new InventoryLargeController();
		inventoryController = new InventoryController();
		armouryBoardController = new ArmouryBoardController();
		
		inventoryExpansionController.Enable();
		inventoryLargeController.Enable();
		inventoryController.Enable();
		armouryBoardController.Enable();
	}

	public void Dispose() {
		inventoryExpansionController.Dispose();
		inventoryLargeController.Dispose();
		inventoryController.Dispose();
		armouryBoardController.Dispose();
	}
}