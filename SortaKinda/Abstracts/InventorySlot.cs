using System.Drawing;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using ImGuiNET;
using ImGuiScene;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Models;
using SortaKinda.System;

namespace SortaKinda.Abstracts;

public unsafe class InventorySlot
{
    private InventoryItem* Item => InventoryController.GetItemForSlot(Type, Index);
    public Item? LuminaData => Item is not null ? LuminaCache<Item>.Instance.GetRow(Item->ItemID) : null;
    private TextureWrap? ItemIcon => LuminaData is not null ? IconCache.Instance.GetIcon(LuminaData.Icon) : null;

    public Pointer<ItemOrderModuleSorterItemEntry> ItemOrderData => InventoryController.GetItemOrderDataForSlot(Type, Index);
    public bool HasItem => Item is not null && Item->ItemID is not 0;
    public SortingRule Rule
    {
        get => SortController.GetRule(ControllingModule.ModuleConfig.Configurations![Type].Rules[Index]);
        set
        {
            ControllingModule.ModuleConfig.Configurations![Type].Rules[Index] = value.Id;
            ControllingModule.SaveConfig();
        }
    }

    public int Index { get; init; }
    public InventoryType Type { get; init; }
    public required InventoryModuleBase ControllingModule { get; init; }

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
        ImGui.Image(ItemIcon.ImGuiHandle, size, Vector2.Zero, Vector2.One, Vector4.One with { W = 0.50f });

        OnItemHovered();
        OnItemLeftClicked();
        OnItemRightClicked();
        OnDragCollision(drawPosition, size);
    }

    private void OnItemHovered()
    {
        if (!ImGui.IsItemHovered()) return;
        
        Rule.DrawTooltip();
    }

    private void OnItemLeftClicked()
    {
        if (!ImGui.IsItemClicked(ImGuiMouseButton.Left)) return;
        if (SortController.SelectedRule is not { } rule || Rule.Equals(rule)) return;
        
        SetToSelectedRule();
    }
    
    private void OnItemRightClicked()
    {
        if (!ImGui.IsItemClicked(ImGuiMouseButton.Right)) return;
        
        Rule = new SortingRule { Id = "Default" };
        ControllingModule.SaveConfig();
    }
    
    private void OnDragCollision(Vector2 drawPosition, Vector2 size)
    {
        if (AreaPaintController.GetDragBounds().IntersectsWith(GetBounds(drawPosition, size)))
        {
            SetToSelectedRule();
        }
    }

    private void SetToSelectedRule()
    {
        if (SortController.SelectedRule is not { } rule || Rule.Equals(rule)) return;
        
        Rule = rule;
        ControllingModule.SaveConfig();
    }
    
    private void DrawFrame(Vector2 drawPosition, Vector2 size)
    {
        var start = ImGui.GetWindowPos() + drawPosition;
        var stop = start + size;
        
        ImGui.GetWindowDrawList().AddRect(start, stop, ImGui.GetColorU32(Rule.Color), 5.0f, ImDrawFlags.None, 2.0f);
    }

    private Rectangle GetBounds(Vector2 drawPosition, Vector2 size)
    {
        var start = ImGui.GetWindowPos() + drawPosition;
        var stop = start + size;

        var startPoint = new Point((int) start.X, (int) start.Y);
        var stopPoint = new Point((int) stop.X, (int) stop.Y);
        var rectSize = new Size(stopPoint.X - startPoint.X, stopPoint.Y - startPoint.Y);

        return new Rectangle(startPoint, rectSize);
    }
}