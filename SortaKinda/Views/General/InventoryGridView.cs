﻿using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using SortaKinda.Interfaces;

namespace SortaKinda.Views.SortControllerViews;

public class InventoryGridView
{
    private const int ItemsPerRow = 5;
    public static Vector2 ItemSpacing => new(5.0f, 5.0f);

    private readonly List<InventorySlotView> inventorySlots = new();

    public InventoryGridView(IInventoryGrid grid, Vector2 position)
    {
        foreach (var inventorySlot in grid.Inventory)
        {
            inventorySlots.Add(new InventorySlotView(inventorySlot, position + GetDrawPositionForIndex(inventorySlot.Slot)));
        }
    }

    public void Draw()
    {
        foreach (var slot in inventorySlots)
        {
            slot.Draw();
        }
    }
    
    private static Vector2 GetDrawPositionForIndex(int index)
    {
        var xPosition = index % ItemsPerRow;
        var yPosition = index / ItemsPerRow;

        var drawPositionX = xPosition * (InventorySlotView.ItemSize.X + ItemSpacing.X);
        var drawPositionY = yPosition * (InventorySlotView.ItemSize.Y + ItemSpacing.Y);

        return new Vector2(drawPositionX, drawPositionY);
    }

    public Vector2 GetGridSize()
    {
        var rowCount = inventorySlots.Count / 5;

        var xSize = ItemsPerRow * (InventorySlotView.ItemSize.X + ItemSpacing.X);
        var ySize = rowCount * (InventorySlotView.ItemSize.Y + ItemSpacing.Y);

        return new Vector2(xSize, ySize);
    }
}