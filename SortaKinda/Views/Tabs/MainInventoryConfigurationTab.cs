using ImGuiNET;
using SortaKinda.Abstracts;
using SortaKinda.Views.Components;

namespace SortaKinda.Views.Tabs;

public class MainInventoryConfigurationTab : IInventoryConfigurationTab
{
    public string TabName => "Main Inventory";
    public bool Enabled => true;

    private readonly MainInventoryView mainInventoryView = new();
    
    public void DrawRuleConfiguration()
    {
        ImGui.Text("Config");
    }
    
    public void DrawInventory()
    {
        mainInventoryView.Draw();
    }
}