using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Utility;
using SortaKinda.System;

namespace SortaKinda.Views.SortControllerViews;

public class QuadInventoryView {
    private readonly List<InventoryGridView> views = [];

    public QuadInventoryView(IReadOnlyList<InventoryGrid> inventoryGrids, Vector2 position) {
        var grid1 = new InventoryGridView(inventoryGrids[0], position + Vector2.Zero);
        var grid2 = new InventoryGridView(inventoryGrids[1], position + new Vector2(grid1.GetGridSize().X + GridSpacing.X, 0.0f));
        var grid3 = new InventoryGridView(inventoryGrids[2], position + new Vector2(0.0f, grid1.GetGridSize().Y + GridSpacing.Y));
        var grid4 = new InventoryGridView(inventoryGrids[3], position + new Vector2(grid1.GetGridSize().X + GridSpacing.X, grid2.GetGridSize().Y + GridSpacing.Y));

        views.Add(grid1);
        views.Add(grid2);
        views.Add(grid3);
        views.Add(grid4);
    }
    
    private static Vector2 GridSpacing => ImGuiHelpers.ScaledVector2(8.0f, 8.0f);

    public void Draw() {
        foreach (var gridView in views) {
            gridView.Draw();
        }
    }
}