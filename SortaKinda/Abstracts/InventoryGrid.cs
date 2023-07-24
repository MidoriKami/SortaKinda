using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using SortaKinda.Interfaces;
using SortaKinda.System;

namespace SortaKinda.Abstracts;

public unsafe class InventoryGrid : IInventoryGrid
{
    private const int ItemsPerRow = 5;

    public InventoryGrid(InventoryType type, InventoryModuleBase sourceModule)
    {
        InventoryType = type;

        InventorySlots = new List<IInventorySlot>();
        foreach (var index in Enumerable.Range(0, ItemsPerPage))
        {
            InventorySlots.Add(new InventorySlot
            {
                Index = index,
                Type = InventoryType,
                ControllingModule = sourceModule
            });
        }
    }

    private static Vector2 ItemSize => new Vector2(80.0f, 80.0f) * ImGuiHelpers.GlobalScale;
    private static Vector2 ItemSpacing => ImGui.GetStyle().ItemSpacing with { Y = 7.0f };
    private int StartIndex => InventoryController.GetInventorySorterStartIndex(InventoryType);
    private int ItemsPerPage => InventoryController.GetInventorySorter(InventoryType)->ItemsPerPage;

    public InventoryType InventoryType { get; init; }
    public float Scale => 0.50f;
    public List<IInventorySlot> InventorySlots { get; set; }
    public Vector2 InventorySize => new Vector2((ItemSize.X + ItemSpacing.X) * ItemsPerRow, (ItemSize.Y + ItemSpacing.Y) * ItemsPerPage / ItemsPerRow) * Scale;

    public void Draw(Vector2 drawPosition)
    {
        foreach (var index in Enumerable.Range(StartIndex, ItemsPerPage))
        {
            var slotDrawPosition = drawPosition + GetDrawPositionForIndex(index - StartIndex);

            var inventorySlot = InventorySlots[index - StartIndex];
            
            inventorySlot.Draw(slotDrawPosition, ItemSize * Scale);
        }
    }

    private Vector2 GetDrawPositionForIndex(int index)
    {
        var xPosition = index % ItemsPerRow;
        var yPosition = index / ItemsPerRow;

        var drawPositionX = xPosition * (ItemSize.X + ItemSpacing.X) * Scale;
        var drawPositionY = yPosition * (ItemSize.Y + ItemSpacing.Y) * Scale;

        return new Vector2(drawPositionX, drawPositionY);
    }
}