using SortaKinda.Abstracts;
using SortaKinda.Models.Enum;
using SortaKinda.System;

namespace SortaKinda.Views.Tabs;

public class MainInventoryConfigurationTab : IInventoryConfigurationTab
{
    public string TabName => "Main Inventory";
    public bool Enabled => true;
    public int TabOrder => 1;
    
    public void DrawInventory() => SortaKindaSystem.ModuleController.GetModule(ModuleName.MainInventory).DrawInventoryGrid();
}