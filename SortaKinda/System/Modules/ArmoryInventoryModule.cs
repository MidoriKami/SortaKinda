using SortaKinda.Abstracts;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.System.Modules;

public class ArmoryInventoryModule : InventoryModuleBase
{
    public override ModuleName ModuleName { get; protected set; } = ModuleName.ArmoryInventory;
    public override IInventoryConfig ModuleConfig { get; protected set; } = new ArmoryInventoryConfig();
    
    
    
    public override void DrawInventoryGrid()
    {
        
    }
}