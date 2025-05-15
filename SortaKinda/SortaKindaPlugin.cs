using System;
using System.Collections.Generic;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ImGuiNET;
using KamiLib.CommandManager;
using KamiLib.Configuration;
using KamiLib.Window;
using KamiToolKit;
using SortaKinda.Addons;
using SortaKinda.Classes;
using SortaKinda.Controllers;
using SortaKinda.Windows;

namespace SortaKinda;

public sealed class SortaKindaPlugin : IDalamudPlugin {
    private uint lastJob = uint.MaxValue;
    private DateTime lastSortCommand = DateTime.MinValue;
    
    public SortaKindaPlugin(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Service>();

        System.NativeController = new NativeController(Service.PluginInterface);
        System.SystemConfig = new SystemConfig();
        System.SortingThreadController = new SortingThreadController();
        System.SortController = new SortController();
        System.ModuleController = new ModuleController();
        System.CommandManager = new CommandManager(Service.PluginInterface, "sortakinda", "sorta");
        System.WindowManager = new WindowManager(Service.PluginInterface);

        System.CommandManager.RegisterCommand(new CommandHandler {
            Delegate = SortCommand,
            ActivationPath = "/sort",
        });
        
        System.WindowManager.AddWindow(new ConfigurationWindow(), WindowFlags.IsConfigWindow);
        
        if (Service.ClientState is { IsLoggedIn: true }) {
            Service.Framework.RunOnFrameworkThread(OnLogin);
        }
        
        System.AddonControllers = new AddonControllers();
        
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.Framework.Update += OnUpdate;
        Service.ClientState.TerritoryChanged += OnZoneChange;
        Service.GameInventory.InventoryChanged += OnInventoryChanged;
    }

    public void Dispose() {
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.Framework.Update -= OnUpdate;
        Service.ClientState.TerritoryChanged -= OnZoneChange;
        Service.GameInventory.InventoryChanged -= OnInventoryChanged;

        System.WindowManager.Dispose();
        System.ModuleController.Dispose();
        System.SortingThreadController.Dispose();
        System.CommandManager.Dispose();
        System.NativeController.Dispose();
        System.AddonControllers.Dispose();
    }
    
    private void OnLogin() {
        System.SystemConfig = LoadConfig();
        System.SystemConfig.UpdateCharacterData();
        SaveConfig();

        System.SortController.Load();
        System.ModuleController.Load();

        if (System.SystemConfig.SortOnLogin) System.ModuleController.Sort();
    }

    private void OnLogout(int type, int code) {
        System.ModuleController.Unload();
        lastJob = uint.MaxValue;
    }

    private void OnUpdate(IFramework framework) {
        if (Service.ClientState is { IsLoggedIn: false }) return;
        if (Service.ClientState is not { LocalPlayer.ClassJob.RowId: var classJobId }) return;
        
        // Don't update modules if the Retainer transfer window is open
        if (Service.GameGui.GetAddonByName("RetainerItemTransferProgress") != nint.Zero) return;
        
        System.ModuleController.Update();

        // Prevent sorting on load, we have a different option for that
        if (lastJob is uint.MaxValue) lastJob = classJobId;

        if (System.SystemConfig.SortOnJobChange && lastJob != classJobId) {
            System.ModuleController.Sort();
            lastJob = classJobId;
        }
        
        System.SortingThreadController.Update();
    }

    private void OnZoneChange(ushort e) {
        if (Service.ClientState is { IsLoggedIn: false }) return;

        if (System.SystemConfig.SortOnZoneChange) System.ModuleController.Sort();
    }

    private void OnInventoryChanged(IReadOnlyCollection<InventoryEventArgs> events) 
        => System.ModuleController.InventoryChanged(events);

    private void SortCommand(params string[] args) {
        var timeSinceLastSort = DateTime.UtcNow - lastSortCommand;
        
        if (timeSinceLastSort.TotalSeconds > 10) {
            System.ModuleController.Sort();
            lastSortCommand = DateTime.UtcNow;
        }
        else {
            Service.ChatGui.PrintError($"Attempted to sort too soon after last sort. Try again in {10 - timeSinceLastSort.Seconds} seconds.");
        }
    }
    
    public static void DrawConfig() {
        var settingsChanged = ImGui.Checkbox("Sort on Item Added", ref System.SystemConfig.SortOnItemAdded);
        settingsChanged |= ImGui.Checkbox("Sort on Item Removed", ref System.SystemConfig.SortOnItemRemoved);
        settingsChanged |= ImGui.Checkbox("Sort on Item Changed", ref System.SystemConfig.SortOnItemChanged);
        settingsChanged |= ImGui.Checkbox("Sort on Item Moved", ref System.SystemConfig.SortOnItemMoved);
        settingsChanged |= ImGui.Checkbox("Sort on Item Merged", ref System.SystemConfig.SortOnItemMerged);
        settingsChanged |= ImGui.Checkbox("Sort on Item Split", ref System.SystemConfig.SortOnItemSplit);
        settingsChanged |= ImGui.Checkbox("Sort on Zone Change", ref System.SystemConfig.SortOnZoneChange);
        settingsChanged |= ImGui.Checkbox("Sort on Job Change", ref System.SystemConfig.SortOnJobChange);
        settingsChanged |= ImGui.Checkbox("Sort on Login", ref System.SystemConfig.SortOnLogin);
        settingsChanged |= ImGui.Checkbox("Reorder Unsorted Items", ref System.SystemConfig.ReorderUnsortedItems);

        if (settingsChanged) {
            SaveConfig();
        }
    }

    private static SystemConfig LoadConfig() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "System.config.json", () => new SystemConfig());

    private static void SaveConfig()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "System.config.json", System.SystemConfig);
}