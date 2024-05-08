using SortaKinda.Controllers;
using SortaKinda.Data.Enums;
using SortaKinda.Windows.ConfigurationWindow.cs.Components;

namespace SortaKinda.Windows.ConfigurationWindow.cs;

public class MainInventoryTab : IInventoryConfigurationTab {
    public string Name => "Main Inventory";
    
    public bool Disabled => false;
    
    public void DrawInventory() 
        => SortaKindaController.ModuleController.DrawModule(ModuleName.MainInventory);
}