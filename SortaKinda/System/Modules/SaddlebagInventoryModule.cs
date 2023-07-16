using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using SortaKinda.Abstracts;
using SortaKinda.Models;
using SortaKinda.Models.Enum;
using Vector2 = System.Numerics.Vector2;

namespace SortaKinda.System.Modules;

public class SaddlebagInventoryModule : InventoryModuleBase
{
    public override ModuleName ModuleName { get; protected set; } = ModuleName.SaddlebagInventory;
    public override IInventoryConfig ModuleConfig { get; protected set; } = new SaddlebagConfig();

    private readonly InventoryGrid inventory1 = new(InventoryType.SaddleBag1);
    private readonly InventoryGrid inventory2 = new(InventoryType.SaddleBag2);
    
    public override void DrawInventoryGrid()
    {
        var region = ImGui.GetContentRegionAvail();

        var firstPosition = new Vector2(0.0f, region.Y / 4.0f);
        var secondPosition = new Vector2(region.X / 2.0f, region.Y / 4.0f);
        
        inventory1.Draw(firstPosition);
        inventory2.Draw(secondPosition);
    }
}