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
using KamiLib.Windows;
using Lumina.Excel.GeneratedSheets;

namespace SortaKinda.Views.Components;

public unsafe class InventoryView
{
    private InventoryType Type { get; set; }
    private Vector2 Offset { get; set; }
    private Vector2 ItemSpacing => ImGui.GetStyle().ItemSpacing with { Y = 7.0f };
    private InventoryContainer* Inventory => InventoryManager.Instance()->GetInventoryContainer(Type);
    private StdVector<Pointer<ItemOrderModuleSorterItemEntry>>* ItemOrderData => (StdVector<Pointer<ItemOrderModuleSorterItemEntry>>*) &UIModule.Instance()->GetItemOrderModule()->InventorySorter->Items;
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
        var windowPos = ImGui.GetWindowPos();
        foreach (var index in Enumerable.Range(0, (int) Inventory->Size))
        {
            var xPosition = index % 5;
            var yPosition = index / 5;
            
            var drawPositionX = xPosition * (ItemSize.X + ItemSpacing.X) * Scale;
            var drawPositionY = yPosition * (ItemSize.Y + ItemSpacing.Y) * Scale;

            var start = windowPos + Offset + new Vector2(drawPositionX, drawPositionY);
            var stop = start + ItemSize * Scale;
            
            ImGui.GetWindowDrawList().AddRect(start, stop, ImGui.GetColorU32(KnownColor.Gray.AsVector4()), 5.0f);
            // ImGui.GetWindowDrawList().AddCircleFilled(start, 5.0f, ImGui.GetColorU32(KnownColor.Gray.AsVector4()));
        }
        
        foreach (var index in Enumerable.Range(0, (int) Inventory->Size))
        {
            var item = Inventory->Items[index];
            var slotData = *ItemOrderData->Span[index].Value;

            var xPosition = slotData.Slot % 5;
            var yPosition = slotData.Slot / 5;

            var drawPositionX = xPosition * (ItemSize.X + ItemSpacing.X) * Scale;
            var drawPositionY = yPosition * (ItemSize.Y + ItemSpacing.Y) * Scale;
            
            if (LuminaCache<Item>.Instance.GetRow(item.ItemID) is not { Icon: var iconId }) continue;
            if (IconCache.Instance.GetIcon(iconId) is not { } icon) continue;
            
            var iconSize = new Vector2(icon.Width, icon.Height) * Scale;

            ImGui.SetCursorPos(Offset + new Vector2(drawPositionX, drawPositionY));
            ImGui.Image(icon.ImGuiHandle, iconSize, Vector2.Zero, Vector2.One, Vector4.One with { W = 0.33f });
        }
    }
}