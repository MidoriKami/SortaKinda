using SortaKinda.Interfaces;
using SortaKinda.Models.Enum;

namespace SortaKinda.Views.Tabs;

public class ArmoryInventoryTab : IInventoryConfigurationTab
{
    public string TabName => "Armory Inventory";
    public bool Enabled => true;
    public void DrawInventory() => SortaKindaPlugin.Controller.ModuleController.DrawModule(ModuleName.ArmoryInventory);
}