using System.Drawing;
using System.Numerics;
using Dalamud.Interface.Utility;
using ImGuiNET;
using SortaKinda.Models.Inventory;
using SortaKinda.System;

namespace SortaKinda.Views.SortControllerViews;

public class InventorySlotView(InventorySlot slot, Vector2 position) {
    public static Vector2 ItemSize => ImGuiHelpers.ScaledVector2(35.0f, 35.0f);

    public void Draw() {
        DrawItem();

        if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) slot.OnLeftClick();
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) slot.OnRightClick();
        if (ImGui.IsItemHovered()) slot.OnHover();
        if (AreaPaintController.GetDragBounds().IntersectsWith(GetBounds(position, ItemSize))) slot.OnDragCollision();

        DrawFrame();
    }

    private void DrawItem() {
        ImGui.SetCursorPos(position);

        if (!slot.HasItem || Service.TextureProvider.GetIcon(slot.ExdItem.Icon) is not { } itemIcon) {
            // Draw Null Image, so we have an item to hover over
            ImGui.Image(nint.Zero, ItemSize);
            return;
        }

        ImGui.Image(itemIcon.ImGuiHandle, ItemSize, Vector2.Zero, Vector2.One, Vector4.One with { W = 0.50f });
    }

    private void DrawFrame() {
        var start = ImGui.GetWindowPos() + position;
        var stop = start + ItemSize;

        ImGui.GetWindowDrawList().AddRect(start, stop, ImGui.GetColorU32(slot.Rule.Color), 5.0f, ImDrawFlags.None, 2.0f);
    }

    private static Rectangle GetBounds(Vector2 drawPosition, Vector2 size) {
        var start = ImGui.GetWindowPos() + drawPosition;
        var stop = start + size;

        var startPoint = new Point((int) start.X, (int) start.Y);
        var stopPoint = new Point((int) stop.X, (int) stop.Y);
        var rectSize = new Size(stopPoint.X - startPoint.X, stopPoint.Y - startPoint.Y);

        return new Rectangle(startPoint, rectSize);
    }
}