// ReSharper disable UnusedMember.Global
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Style;
using Dalamud.Interface.Utility;
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
            new ArmoryInventoryTab(),
            new GeneralConfigurationTab()
        },
        Id = "SortaKindaConfigTabBar"
    };

    public ConfigurationWindow() : base("SortaKinda - Configuration Window")
    {
        Size = new Vector2(840, 636);

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        Flags |= ImGuiWindowFlags.NoResize;

        CommandController.RegisterCommands(this);
    }

    public override bool DrawConditions()
    {
        return Service.ClientState is { IsLoggedIn: true, IsPvP: false, LocalContentId: not 0, LocalPlayer: not null };
    }

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, StyleModelV1.DalamudStandard.WindowPadding * ImGuiHelpers.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, StyleModelV1.DalamudStandard.FramePadding * ImGuiHelpers.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, StyleModelV1.DalamudStandard.CellPadding * ImGuiHelpers.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, StyleModelV1.DalamudStandard.ItemSpacing * ImGuiHelpers.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, StyleModelV1.DalamudStandard.ItemInnerSpacing * ImGuiHelpers.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, StyleModelV1.DalamudStandard.IndentSpacing * ImGuiHelpers.GlobalScale);
    }

    public override void Draw()
    {
        tabBar.Draw();
        areaPaintController.Draw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleVar(6);
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