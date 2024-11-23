using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Style;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.CommandManager;
using KamiLib.Configuration;
using KamiLib.Window;
using SortaKinda.Controllers;
using SortaKinda.Modules;
using SortaKinda.ViewComponents;

namespace SortaKinda.Windows;

public class ConfigurationWindow : Window {
    private readonly AreaPaintController areaPaintController = new();

    private readonly TabBar tabBar = new("SortaKindaConfigTabBar", [
        new MainInventoryTab(),
        new ArmoryInventoryTab(),
        new GeneralConfigurationTab(),
    ]);

    public ConfigurationWindow() : base("SortaKinda - Configuration Window", new Vector2(840.0f, 636.0f), true) {
        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        WindowFlags = WindowFlags.IsConfigWindow;
        
        TitleBarButtons.Add(new TitleBarButton {
            Icon = FontAwesomeIcon.Cog,
            ShowTooltip = () => ImGui.SetTooltip("Open Configuration Manager"),
            Click = _ => System.WindowManager.AddWindow(new ConfigurationManagerWindow(), WindowFlags.OpenImmediately),
            IconOffset = new Vector2(2.0f, 2.0f),
        });

        System.CommandManager.RegisterCommand(new CommandHandler {
            Delegate = OpenConfigWindow,
            ActivationPath = "/",
        });
    }

    public override bool DrawConditions()
        => Service.ClientState is { IsLoggedIn: true };

    public override void PreDraw() 
        => StyleModelV1.DalamudStandard.Push();

    protected override void DrawContents() {
        tabBar.Draw();
        areaPaintController.Draw();
    }

    public override void PostDraw() 
        => StyleModelV1.DalamudStandard.Pop();

    private void OpenConfigWindow(params string[] args) {
        switch (Service.ClientState) {
            case { IsLoggedIn: false }:
                return;

            default:
                UnCollapseOrToggle();
                break;
        }
    }
}

public class MainInventoryTab : IInventoryConfigurationTab {
    public string Name => "Main Inventory";
    
    public bool Disabled => false;
    
    public void DrawInventory() 
        => System.ModuleController.DrawModule(ModuleName.MainInventory);
}

public class ArmoryInventoryTab : IInventoryConfigurationTab {
    public string Name => "Armory Inventory";
    
    public bool Disabled => false;
    
    public void DrawInventory() 
        => System.ModuleController.DrawModule(ModuleName.ArmoryInventory);
}

public class GeneralConfigurationTab : ITabItem {
    public string Name => "General Settings";
    
    public bool Disabled => false;
    
    public void Draw() 
        => SortaKindaPlugin.DrawConfig();
}