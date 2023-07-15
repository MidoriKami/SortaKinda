using System;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Commands;
using KamiLib.Windows;
using Lumina.Excel.GeneratedSheets;
using Vector4 = FFXIVClientStructs.FFXIV.Common.Math.Vector4;

namespace SortaKinda.Views.Components;

public unsafe class InventoryView : IDisposable
{
    private readonly InventoryType inventoryType;
    private InventoryContainer* CurrentInventory => InventoryManager.Instance()->GetInventoryContainer(inventoryType);

    public InventoryView(InventoryType type)
    {
        inventoryType = type;
        
        CommandController.RegisterCommands(this);

        var ptr = new nint(CurrentInventory->GetInventorySlot(0));
        PluginLog.Debug(ptr.ToString("X"));
    }

    public void Draw()
    {
        var itemOdrVector = (StdVector<Pointer<ItemOrderModuleSorterItemEntry>>*) &UIModule.Instance()->GetItemOrderModule()->InventorySorter->Items;
        
        ImGui.Text($"Inventory Size: {CurrentInventory->Size}");
        const float scale = 0.5f;

        var cursorStart = ImGui.GetCursorPos();
        var itemSpacing = ImGui.GetStyle().ItemSpacing with { Y = 7.0f } * 2.0f;
        foreach (var index in Enumerable.Range(0, (int) CurrentInventory->Size))
        {
            var item = CurrentInventory->Items[index];
            var slotData = *itemOdrVector->Span[index].Value;
            
            DebugWindow.Print($"{LuminaCache<Item>.Instance.GetRow(item.ItemID)?.Name}: {slotData.Slot}");

            var xPosition = slotData.Slot % 5;
            var yPosition = slotData.Slot / 5;

            var drawPositionX = xPosition * (80.0f + itemSpacing.X) * scale;
            var drawPositionY = yPosition * (80.0f + itemSpacing.Y) * scale;
            
            if (LuminaCache<Item>.Instance.GetRow(item.ItemID) is not { Icon: var iconId }) continue;
            if (IconCache.Instance.GetIcon(iconId) is not { } icon) continue;
            
            var iconSize = new Vector2(icon.Width, icon.Height) * scale;

            ImGui.SetCursorPos(cursorStart + new Vector2(drawPositionX, drawPositionY));
            ImGui.Image(icon.ImGuiHandle, iconSize, Vector2.Zero, Vector2.One, Vector4.One with { W = 0.50f });
        }
    }

    public void Dispose()
    {
        CommandController.UnregisterCommands(this);
    }
    
    [SingleTierCommandHandler("test", "sort")]
    private void SortFunction()
    {
        var correctType = (StdVector<Pointer<ItemOrderModuleSorterItemEntry>>*) &UIModule.Instance()->GetItemOrderModule()->InventorySorter->Items;

        (correctType->First[1].Value->Slot, correctType->First[0].Value->Slot) = (correctType->First[0].Value->Slot, correctType->First[1].Value->Slot);
    }

}