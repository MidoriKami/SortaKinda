using SortaKinda.Interfaces;
using SortaKinda.Models.Enums;
using SortaKinda.System;

namespace SortaKinda.Views.Tabs;

public class ArmoryInventoryTab : IInventoryConfigurationTab {
    public string Name => "Armory Inventory";
    
    public bool Disabled => false;
    
    public void DrawInventory() => SortaKindaController.ModuleController.DrawModule(ModuleName.ArmoryInventory);
}