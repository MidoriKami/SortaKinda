using SortaKinda.Interfaces;
using SortaKinda.Models.Enum;

namespace SortaKinda.Views.Tabs;

public class MainInventoryTab : IInventoryConfigurationTab
{
    public string TabName => "Main Inventory";
    public bool Enabled => true;
    public void DrawInventory() => SortaKindaPlugin.Controller.ModuleController.DrawModule(ModuleName.MainInventory);
}