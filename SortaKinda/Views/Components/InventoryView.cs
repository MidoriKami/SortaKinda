using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;

namespace SortaKinda.Views.Components;

public unsafe class InventoryView
{
    private InventoryType Type { get; set; }
    private Vector2 Offset { get; set; }
    private Vector2 ItemSpacing => ImGui.GetStyle().ItemSpacing with { Y = 7.0f };
    private InventoryManager* Inventory => InventoryManager.Instance();
    private ItemOrderModuleSorter* InventorySorter => GetSorterForType(Type);
    private StdVector<Pointer<ItemOrderModuleSorterItemEntry>>* ItemOrderData => (StdVector<Pointer<ItemOrderModuleSorterItemEntry>>*) &InventorySorter->Items;
    private Vector2 ItemSize => new(80.0f, 80.0f);
    private float Scale => 0.50f;
    private int NumColumns => 5;
    private int NumRows => 7;
    
    public Vector2 Size => new Vector2((ItemSize.X + ItemSpacing.X) * NumColumns, (ItemSize.Y + ItemSpacing.Y) * NumRows) * Scale;
    
    public InventoryView(InventoryType type, Vector2 offset)
    {
        Type = type;
        Offset = offset;
    }

    public void Draw()
    {
        foreach (var index in Enumerable.Range(0, InventorySorter->ItemsPerPage))
        {
            DrawInventoryBox(index);
        }

        foreach (var index in Enumerable.Range(GetStartIndex(), InventorySorter->ItemsPerPage))
        {
            var slotData = *ItemOrderData->Span[index].Value;
            var item = Inventory->GetInventoryContainer(InventoryType.Inventory1 + slotData.Page)->GetInventorySlot(slotData.Slot);
            
            // Since we draw one page at a time, we need have the same offset
            DrawItem(item->ItemID, index - GetStartIndex());
        }
    }

    private void DrawItem(uint itemId, int slot)
    {
        var xPosition = slot % 5;
        var yPosition = slot / 5;
        
        var drawPositionX = xPosition * (ItemSize.X + ItemSpacing.X) * Scale;
        var drawPositionY = yPosition * (ItemSize.Y + ItemSpacing.Y) * Scale;

        if (LuminaCache<Item>.Instance.GetRow(itemId) is not { Icon: var iconId }) return;
        if (IconCache.Instance.GetIcon(iconId) is not { } icon) return;
        
        var iconSize = new Vector2(icon.Width, icon.Height) * Scale;
        
        ImGui.SetCursorPos(Offset + new Vector2(drawPositionX, drawPositionY));
        ImGui.Image(icon.ImGuiHandle, iconSize, Vector2.Zero, Vector2.One, Vector4.One with { W = 0.33f });
    }

    private void DrawInventoryBox(int slot)
    {
        var windowPos = ImGui.GetWindowPos();
        
        var xPosition = slot % 5;
        var yPosition = slot / 5;
            
        var drawPositionX = xPosition * (ItemSize.X + ItemSpacing.X) * Scale;
        var drawPositionY = yPosition * (ItemSize.Y + ItemSpacing.Y) * Scale;

        var start = windowPos + Offset + new Vector2(drawPositionX, drawPositionY);
        var stop = start + ItemSize * Scale;
            
        ImGui.GetWindowDrawList().AddRect(start, stop, ImGui.GetColorU32(KnownColor.Gray.AsVector4()), 5.0f);
    }

    private int GetStartIndex() => Type switch
    {
        InventoryType.Inventory1 => 0,
        InventoryType.Inventory2 => InventorySorter->ItemsPerPage,
        InventoryType.Inventory3 => InventorySorter->ItemsPerPage * 2,
        InventoryType.Inventory4 => InventorySorter->ItemsPerPage * 3,
        _ => throw new Exception($"Type Not Implemented: {Type}")
    };
    
    private ItemOrderModuleSorter* GetSorterForType(InventoryType type) => type switch
    {
        InventoryType.Inventory1 => UIModule.Instance()->GetItemOrderModule()->InventorySorter,
        InventoryType.Inventory2 => UIModule.Instance()->GetItemOrderModule()->InventorySorter,
        InventoryType.Inventory3 => UIModule.Instance()->GetItemOrderModule()->InventorySorter,
        InventoryType.Inventory4 => UIModule.Instance()->GetItemOrderModule()->InventorySorter,
        _ => throw new Exception($"Type Not Implemented: {type}")
    };
}