using System;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using SortaKinda.Abstracts;
using SortaKinda.Views.Components;

namespace SortaKinda.Views.Tabs;

public class MainInventoryConfigurationTab : IInventoryConfigurationTab, IDisposable
{
    public string TabName => "Main Inventory";
    public bool Enabled => true;

    private readonly InventoryView inventoryView = new(InventoryType.Inventory1);
    
    public void DrawRuleConfiguration()
    {
        ImGui.Text("Config");
    }
    
    public void DrawInventory()
    {
        ImGui.Text("Inventory");
        
        inventoryView.Draw();
    }
    
    public void Dispose()
    {
        inventoryView.Dispose();
    }
}