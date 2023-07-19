using SortaKinda.Abstracts;
using SortaKinda.Models.Enum;
using SortaKinda.System;

namespace SortaKinda.Views.Tabs;

public class ArmoryInventoryConfigurationTab : IInventoryConfigurationTab
{
    public string TabName => "Armory Inventory";
    public bool Enabled => true;
    public int TabOrder => 2;

    public void DrawInventory() => SortaKindaSystem.ModuleController.GetModule(ModuleName.ArmoryInventory).DrawInventoryGrid();
}