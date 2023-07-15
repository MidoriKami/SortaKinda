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
    
    // [SingleTierCommandHandler("test", "sort")]
    // private void SortFunction()
    // {
    //     var correctType = (StdVector<Pointer<ItemOrderModuleSorterItemEntry>>*) &UIModule.Instance()->GetItemOrderModule()->InventorySorter->Items;
    //
    //     (correctType->First[1].Value->Slot, correctType->First[0].Value->Slot) = (correctType->First[0].Value->Slot, correctType->First[1].Value->Slot);
    // }

}