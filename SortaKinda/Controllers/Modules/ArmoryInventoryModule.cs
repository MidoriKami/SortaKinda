using SortaKinda.Models.Configuration;
using SortaKinda.Models.Enum;

namespace SortaKinda.System.Modules;

public class ArmoryInventoryModule : ModuleBase
{
    public override ModuleName ModuleName { get; protected set; } = ModuleName.ArmoryInventory;
    protected override IModuleConfig ModuleConfig { get; set; } = new ArmoryConfig();
    
    protected override void Initialize()
    {
        
    }

    public override void Draw()
    {
        
    }
    
    protected override void InternalUpdate()
    {
    }
    
    protected override void DoSort()
    {
        
    }
}