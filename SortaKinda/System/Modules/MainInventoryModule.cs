using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using SortaKinda.Abstracts;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.System.Modules;

public class MainInventoryModule : InventoryModuleBase
{
    public override ModuleName ModuleName { get; protected set; } = ModuleName.MainInventory;
    public override IInventoryConfig ModuleConfig { get; protected set; } = new MainInventoryConfig();

    private readonly InventoryGrid inventory1 = new(InventoryType.Inventory1);
    private readonly InventoryGrid inventory2 = new(InventoryType.Inventory2);
    private readonly InventoryGrid inventory3 = new(InventoryType.Inventory3);
    private readonly InventoryGrid inventory4 = new(InventoryType.Inventory4);
    
    public override void DrawInventoryGrid()
    {
        var region = ImGui.GetContentRegionAvail();

        var firstPosition = new Vector2(0.0f, 0.0f);
        var secondPosition = new Vector2(region.X / 2.0f, 0.0f);
        var thirdPosition = new Vector2(0.0f, region.Y / 2.0f);
        var fourthPosition = new Vector2(region.X / 2.0f, region.Y / 2.0f);
        
        inventory1.Draw(firstPosition);
        inventory2.Draw(secondPosition);
        inventory3.Draw(thirdPosition);
        inventory4.Draw(fourthPosition);
    }
    
    // [SingleTierCommandHandler("test", "sort")]
    // private void SortFunction()
    // {
    //     var correctType = (StdVector<Pointer<ItemOrderModuleSorterItemEntry>>*) &UIModule.Instance()->GetItemOrderModule()->InventorySorter->Items;
    //
    //     (correctType->First[1].Value->Slot, correctType->First[0].Value->Slot) = (correctType->First[0].Value->Slot, correctType->First[1].Value->Slot);
    // }
}