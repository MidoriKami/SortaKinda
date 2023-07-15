using ImGuiNET;
using SortaKinda.Abstracts;
using SortaKinda.Views.Components;

namespace SortaKinda.Views.Tabs;

public class ArmoryInventoryConfigurationTab : IInventoryConfigurationTab
{
    public string TabName => "Armory Inventory";
    public bool Enabled => true;

    private readonly ArmoryInventoryView armoryInventoryView = new();
    
    public void DrawRuleConfiguration()
    {
        ImGui.Text("Config");
    }
    
    public void DrawInventory()
    {
        armoryInventoryView.Draw();
    }
}