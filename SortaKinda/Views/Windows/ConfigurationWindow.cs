using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.Commands;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace SortaKinda.Views.Windows;

public class ConfigurationWindow : Window, IDisposable
{
    private readonly IEnumerable<ITabItem> tabs;

    public ConfigurationWindow() : base("SortaKinda - Configuration Window")
    {
        tabs = new List<ITabItem>(Reflection.ActivateOfInterface<ITabItem>());
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(880, 620),
            MaximumSize = new Vector2(9999,9999),
        };
        
        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

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
    
    public void Dispose()
    {
        foreach (var tab in tabs.OfType<IDisposable>())
        {
            tab.Dispose();
        }
    }
}