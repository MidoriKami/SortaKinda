using SortaKinda.Controllers;
using SortaKinda.Data.Enums;
using SortaKinda.Windows.ConfigurationWindow.cs.Components;

namespace SortaKinda.Windows.ConfigurationWindow.cs;

public class ArmoryInventoryTab : IInventoryConfigurationTab {
    public string Name => "Armory Inventory";
    
    public bool Disabled => false;
    
    public void DrawInventory() 
        => SortaKindaController.ModuleController.DrawModule(ModuleName.ArmoryInventory);
}