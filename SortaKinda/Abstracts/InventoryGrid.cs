using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using SortaKinda.System;

namespace SortaKinda.Abstracts;

public abstract unsafe class InventoryGrid
{
    private const int ItemsPerRow = 5;

    protected Vector2 ItemSize => new Vector2(80.0f, 80.0f) * ImGuiHelpers.GlobalScale;
    protected float ItemScale => 0.50f;
    protected Vector2 ItemSpacing => ImGui.GetStyle().ItemSpacing with { Y = 7.0f };
    
    public InventoryType InventoryType { get; init; }
    public List<InventorySlot> InventorySlots { get; set; }

    protected InventoryGrid(InventoryType type)
    {
        InventoryType = type;

        InventorySlots = new List<InventorySlot>();
        var itemsPerPage = InventoryController.GetInventorySorter(InventoryType)->ItemsPerPage;
        foreach (var index in Enumerable.Range(0, itemsPerPage))
        {
            InventorySlots.Add(new InventorySlot
            {
                Index = index,
                Type = InventoryType,
            });
        }
    }

    public void Draw(Vector2 drawPosition)
    {
        var startIndex = InventoryController.GetInventorySorterStartIndex(InventoryType);
        var itemsPerPage = InventoryController.GetInventorySorter(InventoryType)->ItemsPerPage;
        
        foreach(var index in Enumerable.Range(startIndex, itemsPerPage))
        {
            var slotDrawPosition = drawPosition + GetDrawPositionForIndex(index - startIndex);
            
            InventorySlots[index - startIndex].Draw(slotDrawPosition, ItemSize * ItemScale);
        }
    }

    private Vector2 GetDrawPositionForIndex(int index)
    {
        var xPosition = index % ItemsPerRow;
        var yPosition = index / ItemsPerRow;
        
        var drawPositionX = xPosition * (ItemSize.X + ItemSpacing.X) * ItemScale;
        var drawPositionY = yPosition * (ItemSize.Y + ItemSpacing.Y) * ItemScale;

        return new Vector2(drawPositionX, drawPositionY);
    }
}