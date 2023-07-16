using System.Drawing;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using ImGuiScene;
using KamiLib.Caching;
using KamiLib.Utilities;
using KamiLib.Windows;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.System;

namespace SortaKinda.Abstracts;

public unsafe class InventorySlot
{
    private InventoryItem* Item => InventoryController.GetItemForSlot(Type, Index);
    private Item? LuminaData => LuminaCache<Item>.Instance.GetRow(Item->ItemID);
    private TextureWrap? ItemIcon => LuminaData is not null ? IconCache.Instance.GetIcon(LuminaData.Icon) : null;
    
    public int Index { get; init; }
    public InventoryType Type { get; init; }
    public Vector4 BorderColor { get; set; } = KnownColor.Aqua.AsVector4();

    public void Draw(Vector2 drawPosition, Vector2 size)
    {
        DrawItem(drawPosition, size);
        DrawFrame(drawPosition, size);
    }

    private void DrawItem(Vector2 drawPosition, Vector2 size)
    {
        if (Item is null) return;
        if (LuminaData is null) return;
        if (ItemIcon is null) return;
        
        ImGui.SetCursorPos(drawPosition);
        ImGui.Image(ItemIcon.ImGuiHandle, size, Vector2.Zero, Vector2.One, Vector4.One ); // with { W = 0.40f }
    }

    private void DrawFrame(Vector2 drawPosition, Vector2 size)
    {
        var start = ImGui.GetWindowPos() + drawPosition;
        var stop = start + size;
        
        ImGui.GetWindowDrawList().AddRect(start, stop, ImGui.GetColorU32(BorderColor), 5.0f);
    }
}