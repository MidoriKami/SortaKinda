using SortaKinda.Interfaces;
using SortaKinda.Models.Enums;
using SortaKinda.System;

namespace SortaKinda.Views.Tabs;

public class ArmoryInventoryTab : IInventoryConfigurationTab
{
    public string TabName => "Armory Inventory";
    public bool Enabled => true;
    public void DrawInventory()
    {
        SortaKindaController.ModuleController.DrawModule(ModuleName.ArmoryInventory);
    }
}