using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.Commands;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace SortaKinda.Views.Windows;

public class ConfigurationWindow : Window
{
    private readonly IEnumerable<ITabItem> tabs;

    public ConfigurationWindow() : base("SortaKinda - Configuration Window")
    {
        tabs = new List<ITabItem>(Reflection.ActivateOfInterface<ITabItem>());

        Size = new Vector2(880, 690);
        
        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        Flags |= ImGuiWindowFlags.NoResize;

        IsOpen = true;
        
        CommandController.RegisterCommands(this);
    }
    
    public override void Draw()
    {
        if (ImGui.BeginTabBar("##SortaKindaTabBar"))
        {
            foreach (var tab in tabs)
            {
                if (ImGui.BeginTabItem(tab.TabName))
                {
                    tab.Draw();
                    ImGui.EndTabItem();
                }
            }
            
            ImGui.EndTabBar();
        }
    }
    
    [BaseCommandHandler("OpenConfigWindow")]
    public void OpenConfigWindow()
    {
        if (Service.ClientState.IsPvP) return;
            
        Toggle();
    }
}