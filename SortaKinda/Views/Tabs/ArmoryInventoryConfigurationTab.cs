using ImGuiNET;
using SortaKinda.Abstracts;
using SortaKinda.Models.Enum;
using SortaKinda.System;

namespace SortaKinda.Views.Tabs;

public class ArmoryInventoryConfigurationTab : IInventoryConfigurationTab
{
    public string TabName => "Armory Inventory";
    public bool Enabled => true;

    public void DrawRuleConfiguration()
    {
        ImGui.Text("Config");
    }

    public void DrawInventory() => SortaKindaSystem.ModuleController.GetModule(ModuleName.ArmoryInventory).DrawInventoryGrid();
}