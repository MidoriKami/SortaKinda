// ReSharper disable UnusedMember.Global
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.ChatCommands;
using KamiLib.Commands;
using KamiLib.Interfaces;
using KamiLib.Windows;
using SortaKinda.System;
using SortaKinda.Views.Tabs;

namespace SortaKinda.Views.Windows;

public class ConfigurationWindow : Window
{
    private readonly AreaPaintController areaPaintController = new();

    private readonly TabBar tabBar = new()
    {
        TabItems = new List<ITabItem>
        {
            new MainInventoryTab(),
            new GeneralConfigurationTab(),
        },
        Id = "SortaKindaConfigTabBar",
    };

    public ConfigurationWindow() : base("SortaKinda - Configuration Window")
    {
        Size = new Vector2(840, 636);

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        Flags |= ImGuiWindowFlags.NoResize;

        // todo: remove this
        IsOpen = true;

        CommandController.RegisterCommands(this);
    }

    public override bool DrawConditions()
        => Service.ClientState is { IsLoggedIn: true, IsPvP: false, LocalContentId: not 0, LocalPlayer: not null };

    public override void Draw()
    {        
        tabBar.Draw();
        areaPaintController.Draw();
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