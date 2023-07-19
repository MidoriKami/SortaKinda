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
        get => ControllingModule.ModuleConfig.Configurations![Type].Rules[Index];
        private set => ControllingModule.ModuleConfig.Configurations![Type].Rules[Index] = value;
    }

    public int Index { get; init; }
    public InventoryType Type { get; init; }
    public required InventoryModuleBase ControllingModule { get; set; }

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

        if (ImGui.IsItemHovered())
        {
            Rule.DrawTooltip();
        }
        
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
            if (SortController.SelectedRule is { } rule)
            {
                Rule = rule;
                ControllingModule.SaveConfig();
            }
        }

        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            Rule = new SortingRule
            {
                Id = "Default"
            };
            ControllingModule.SaveConfig();
        }
    }

    private void DrawFrame(Vector2 drawPosition, Vector2 size)
    {
        var start = ImGui.GetWindowPos() + drawPosition;
        var stop = start + size;
        
        ImGui.GetWindowDrawList().AddRect(start, stop, ImGui.GetColorU32(Rule.Color), 5.0f, ImDrawFlags.None, 2.0f);
    }
}