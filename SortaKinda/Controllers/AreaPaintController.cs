using System;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using KamiLib;
using KamiLib.Utility;
using SortaKinda.Views.Windows;

namespace SortaKinda.System;

// I'm sorry this is super weird... sorta a singleton, but also not...
public class AreaPaintController
{
    private static bool _dragStarted;
    private static Vector2 _dragStartPosition = Vector2.Zero;
    private static Vector2 _dragStopPosition = Vector2.Zero;
    private Vector2 lastWindowPosition = Vector2.Zero;

    public static Rectangle GetDragBounds()
    {
        if (_dragStarted is false) return Rectangle.Empty;

        var startPoint = new Point((int) _dragStartPosition.X, (int) _dragStartPosition.Y);
        var stopPoint = new Point((int) _dragStopPosition.X, (int) _dragStopPosition.Y);
        var size = new Size(Math.Abs(stopPoint.X - startPoint.X), Math.Abs(stopPoint.Y - startPoint.Y));

        var minX = Math.Min(startPoint.X, stopPoint.X);
        var minY = Math.Min(startPoint.Y, stopPoint.Y);

        return new Rectangle(new Point(minX, minY), size);
    }

    public void Draw()
    {
        if (!ShouldDraw())
        {
            _dragStarted = false;
            return;
        }

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !_dragStarted)
        {
            _dragStartPosition = ImGui.GetMousePos();
            _dragStopPosition = ImGui.GetMousePos();
            _dragStarted = true;
        }

        if (ImGui.IsMouseDragging(ImGuiMouseButton.Left) && _dragStarted)
        {
            _dragStopPosition = ImGui.GetMousePos();

            if (SortaKindaController.SortController.SelectedRule is not { Color: var ruleColor }) return;
            ImGui.GetWindowDrawList().AddRect(_dragStartPosition, _dragStopPosition, ImGui.GetColorU32(ruleColor), 0.0f, ImDrawFlags.None, 3.0f);
            ImGui.GetWindowDrawList().AddRectFilled(_dragStartPosition, _dragStopPosition, ImGui.GetColorU32(ruleColor with { W = 0.33f }));
        }
        else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            _dragStarted = false;
        }
    }

    private bool ShouldDraw()
    {
        if (KamiCommon.WindowManager.GetWindowOfType<ConfigurationWindow>() is not { IsFocused: true }) return false;
        if (!Bound.IsCursorInWindow() || Bound.IsCursorInWindowHeader()) return false;
        if (IsWindowMoving()) return false;

        return true;
    }

    private bool IsWindowMoving()
    {
        var currentPosition = ImGui.GetWindowPos();
        var windowMoved = currentPosition != lastWindowPosition;

        lastWindowPosition = currentPosition;
        return windowMoved;
    }
}