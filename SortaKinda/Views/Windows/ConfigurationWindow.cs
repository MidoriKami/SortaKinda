using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.ChatCommands;
using KamiLib.Commands;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using SortaKinda.Abstracts;
using SortaKinda.System;
using SortaKinda.Views.Tabs;

namespace SortaKinda.Views.Windows;

public class ConfigurationWindow : Window
{
    private readonly List<ITabItem> tabs = new();
    public AreaPaintController AreaPaintController = new();

    public ConfigurationWindow() : base("SortaKinda - Configuration Window")
    {
        tabs.AddRange(Reflection.ActivateOfInterface<IInventoryConfigurationTab>().OrderBy(tab => tab.TabOrder));
        tabs.Add(new GeneralConfigurationTab());

        Size = new Vector2(880, 690);
        
        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        Flags |= ImGuiWindowFlags.NoResize;

        CommandController.RegisterCommands(this);
    }

    public override bool DrawConditions()
    {
        if (!Service.ClientState.IsLoggedIn) return false;
        if (Service.ClientState.IsPvP) return false;

        return true;
    }

    public override void Draw()
    {
        AreaPaintController.Draw();
        
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
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP)
        {
            Chat.PrintError("The configuration menu cannot be opened while in a PvP area");
            return;
        }
            
        Toggle();
    }
}