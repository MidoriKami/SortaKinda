using SortaKinda.Abstracts;
using SortaKinda.Models.Enum;
using SortaKinda.System;

namespace SortaKinda.Views.Tabs;

public class SaddlebagInventoryConfigurationTab : IInventoryConfigurationTab
{
    public string TabName => "Saddlebag Inventory";
    public bool Enabled => true;
    public int TabOrder => 3;

    public void DrawRuleConfiguration()
    {
        
    }

    public void DrawInventory() => SortaKindaSystem.ModuleController.GetModule(ModuleName.SaddlebagInventory).DrawInventoryGrid();
}