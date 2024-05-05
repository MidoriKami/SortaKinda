using SortaKinda.Interfaces;
using SortaKinda.Models.Enums;
using SortaKinda.System;

namespace SortaKinda.Views.Tabs;

public class MainInventoryTab : IInventoryConfigurationTab {
    public string Name => "Main Inventory";
    
    public bool Disabled => false;
    
    public void DrawInventory() 
        => SortaKindaController.ModuleController.DrawModule(ModuleName.MainInventory);
}