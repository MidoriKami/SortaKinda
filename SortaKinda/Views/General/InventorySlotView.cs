using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Caching;
using SortaKinda.Interfaces;
using SortaKinda.System;

namespace SortaKinda.Views.SortControllerViews;

public class InventorySlotView
{
    public static Vector2 ItemSize => ImGuiHelpers.ScaledVector2(35.0f, 35.0f);

    private readonly IInventorySlot inventorySlot;
    private readonly Vector2 drawPosition;

    public InventorySlotView(IInventorySlot slot, Vector2 position)
    {
        inventorySlot = slot;
        drawPosition = position;
    }

    public void Draw()
    {
        DrawItem();
        DrawFrame();
        
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) inventorySlot.OnLeftClick();
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) inventorySlot.OnRightClick();
        if (ImGui.IsItemHovered()) inventorySlot.OnHover();
        if (AreaPaintController.GetDragBounds().IntersectsWith(GetBounds(drawPosition, ItemSize))) inventorySlot.OnDragCollision();
    }

    private void DrawItem()
    {
        if (!inventorySlot.HasItem) return;
        if (IconCache.Instance.GetIcon(inventorySlot.Item.Icon) is not { } itemIcon) return;
        
        ImGui.SetCursorPos(drawPosition);
        ImGui.Image(itemIcon.ImGuiHandle, ItemSize, Vector2.Zero, Vector2.One, Vector4.One with { W = 0.33f });
    }
    
    private void DrawFrame()
    {
        var start = ImGui.GetWindowPos() + drawPosition;
        var stop = start + ItemSize;
    
        ImGui.GetWindowDrawList().AddRect(start, stop, ImGui.GetColorU32(inventorySlot.Rule.Color), 5.0f, ImDrawFlags.None, 2.0f);
    }
    
    private static Rectangle GetBounds(Vector2 drawPosition, Vector2 size)
    {
        var start = ImGui.GetWindowPos() + drawPosition;
        var stop = start + size;
    
        var startPoint = new Point((int) start.X, (int) start.Y);
        var stopPoint = new Point((int) stop.X, (int) stop.Y);
        var rectSize = new Size(stopPoint.X - startPoint.X, stopPoint.Y - startPoint.Y);
    
        return new Rectangle(startPoint, rectSize);
    }
}