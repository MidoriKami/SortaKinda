using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.Views.Components;

public class MainInventoryView
{
    private readonly InventoryView inventory1;
    private readonly InventoryView inventory2;
    private readonly InventoryView inventory3;
    private readonly InventoryView inventory4;
    
    public MainInventoryView()
    {
        inventory1 = new InventoryView(InventoryType.Inventory1, new Vector2(0.0f, 3.0f));
        inventory2 = new InventoryView(InventoryType.Inventory2, inventory1.Size with { Y = 0 } + new Vector2(10.0f, 3.0f));
        inventory3 = new InventoryView(InventoryType.Inventory3, inventory1.Size with { X = 0 }  + new Vector2(0.0f, 13.0f));
        inventory4 = new InventoryView(InventoryType.Inventory4, inventory1.Size + new Vector2(10.0f, 13.0f));
    }
    
    public void Draw()
    {
        inventory1.Draw();
        inventory2.Draw();
        inventory3.Draw();
        inventory4.Draw();
    }

    // private void DrawInventory(InventoryType type, Vector2 offset)
    // {
    //     var itemOdrVector = (StdVector<Pointer<ItemOrderModuleSorterItemEntry>>*) &UIModule.Instance()->GetItemOrderModule()->InventorySorter->Items;
    //
    //     var currentInventory = InventoryManager.Instance()->GetInventoryContainer(type);
    //     
    //     const float scale = 0.5f;
    //
    //     var cursorStart = ImGui.GetCursorPos() + offset;
    //     var itemSpacing = ImGui.GetStyle().ItemSpacing with { Y = 7.0f } * 2.0f;
    //     foreach (var index in Enumerable.Range(0, (int) currentInventory->Size))
    //     {
    //         var item = currentInventory->Items[index];
    //         var slotData = *itemOdrVector->Span[index].Value;
    //         
    //         DebugWindow.Print($"{LuminaCache<Item>.Instance.GetRow(item.ItemID)?.Name}: {slotData.Slot}");
    //
    //         var xPosition = slotData.Slot % 5;
    //         var yPosition = slotData.Slot / 5;
    //
    //         var drawPositionX = xPosition * (80.0f + itemSpacing.X) * scale;
    //         var drawPositionY = yPosition * (80.0f + itemSpacing.Y) * scale;
    //         
    //         if (LuminaCache<Item>.Instance.GetRow(item.ItemID) is not { Icon: var iconId }) continue;
    //         if (IconCache.Instance.GetIcon(iconId) is not { } icon) continue;
    //         
    //         var iconSize = new Vector2(icon.Width, icon.Height) * scale;
    //
    //         ImGui.SetCursorPos(cursorStart + new Vector2(drawPositionX, drawPositionY));
    //         ImGui.Image(icon.ImGuiHandle, iconSize, Vector2.Zero, Vector2.One, Vector4.One with { W = 0.50f });
    //     }
    // }

    //
    // [SingleTierCommandHandler("test", "sort")]
    // private void SortFunction()
    // {
    //     var correctType = (StdVector<Pointer<ItemOrderModuleSorterItemEntry>>*) &UIModule.Instance()->GetItemOrderModule()->InventorySorter->Items;
    //
    //     (correctType->First[1].Value->Slot, correctType->First[0].Value->Slot) = (correctType->First[0].Value->Slot, correctType->First[1].Value->Slot);
    // }

}