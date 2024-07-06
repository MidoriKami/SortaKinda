using System;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using KamiLib.Classes;
using SortaKinda.Windows;

namespace SortaKinda.Controllers;

public class AreaPaintController {
    private static bool dragStarted;
    private static Vector2 dragStartPosition = Vector2.Zero;
    private static Vector2 dragStopPosition = Vector2.Zero;
    private Vector2 lastWindowPosition = Vector2.Zero;

    public static Rectangle GetDragBounds() {
        if (dragStarted is false) return Rectangle.Empty;

        var startPoint = new Point((int) dragStartPosition.X, (int) dragStartPosition.Y);
        var stopPoint = new Point((int) dragStopPosition.X, (int) dragStopPosition.Y);
        var size = new Size(Math.Abs(stopPoint.X - startPoint.X), Math.Abs(stopPoint.Y - startPoint.Y));

        var minX = Math.Min(startPoint.X, stopPoint.X);
        var minY = Math.Min(startPoint.Y, stopPoint.Y);

        return new Rectangle(new Point(minX, minY), size);
    }

    public void Draw() {
        if (!ShouldDraw()) {
            dragStarted = false;
            return;
        }

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !dragStarted) {
            dragStartPosition = ImGui.GetMousePos();
            dragStopPosition = ImGui.GetMousePos();
            dragStarted = true;
        }

        if (ImGui.IsMouseDragging(ImGuiMouseButton.Left) && dragStarted) {
            dragStopPosition = ImGui.GetMousePos();

            if (System.SortController.SelectedRule is not { Color: var ruleColor }) return;
            ImGui.GetWindowDrawList().AddRect(dragStartPosition, dragStopPosition, ImGui.GetColorU32(ruleColor), 0.0f, ImDrawFlags.None, 3.0f);
            ImGui.GetWindowDrawList().AddRectFilled(dragStartPosition, dragStopPosition, ImGui.GetColorU32(ruleColor with { W = 0.33f }));
        }
        else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left)) {
            dragStarted = false;
        }
    }

    private bool ShouldDraw() {
        if (System.WindowManager.GetWindow<ConfigurationWindow>() is not { IsFocused: true }) return false;
        if (!WindowBounds.IsCursorInWindow() || WindowBounds.IsCursorInWindowHeader()) return false;
        if (IsWindowMoving()) return false;

        return true;
    }

    private bool IsWindowMoving() {
        var currentPosition = ImGui.GetWindowPos();
        var windowMoved = currentPosition != lastWindowPosition;

        lastWindowPosition = currentPosition;
        return windowMoved;
    }
}