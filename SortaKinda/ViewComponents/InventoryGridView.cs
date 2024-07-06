using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Utility;
using SortaKinda.Classes;

namespace SortaKinda.ViewComponents;

public class InventoryGridView {
    private const int ItemsPerRow = 5;

    private readonly List<InventorySlotView> inventorySlots = [];

    public InventoryGridView(InventoryGrid grid, Vector2 position) {
        foreach (var inventorySlot in grid.Inventory) {
            inventorySlots.Add(new InventorySlotView(inventorySlot, position + GetDrawPositionForIndex(inventorySlot.Slot)));
        }
    }
    
    private static Vector2 ItemSpacing => ImGuiHelpers.ScaledVector2(5.0f, 5.0f);

    public void Draw() {
        foreach (var slot in inventorySlots) {
            slot.Draw();
        }
    }

    private static Vector2 GetDrawPositionForIndex(int index) {
        var xPosition = index % ItemsPerRow;
        var yPosition = index / ItemsPerRow;

        var drawPositionX = xPosition * (InventorySlotView.ItemSize.X + ItemSpacing.X);
        var drawPositionY = yPosition * (InventorySlotView.ItemSize.Y + ItemSpacing.Y);

        return new Vector2(drawPositionX, drawPositionY);
    }

    public Vector2 GetGridSize() {
        var rowCount = inventorySlots.Count / 5;

        var xSize = ItemsPerRow * (InventorySlotView.ItemSize.X + ItemSpacing.X);
        var ySize = rowCount * (InventorySlotView.ItemSize.Y + ItemSpacing.Y);

        return new Vector2(xSize, ySize);
    }

    public static float GetGridWidth() 
        => InventorySlotView.ItemSize.X * ItemsPerRow + ItemSpacing.X * ( ItemsPerRow - 1 );
}