using System;
using System.Collections.Generic;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using Dalamud.Plugin.Services;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.CommandManager;
using KamiLib.Configuration;
using KamiLib.Window;
using SortaKinda.Views.Windows;

namespace SortaKinda.System;

public class SystemConfig : CharacterConfiguration {
    public bool SortOnItemAdded = true;
    public bool SortOnItemRemoved = true;
    public bool SortOnItemChanged;
    public bool SortOnItemMoved;
    public bool SortOnItemMerged;
    public bool SortOnItemSplit;
    public bool SortOnZoneChange = true;
    public bool SortOnJobChange = true;
    public bool SortOnLogin = true;
    public bool ReorderUnsortedItems;
}

public class SortaKindaController : IDisposable {
    public static ModuleController ModuleController = null!;
    public static SortController SortController = null!;
    public static SystemConfig SystemConfig = null!;
    public static SortingThreadController SortingThreadController = null!;
    public static CommandManager CommandManager = null!;
    public static WindowManager WindowManager = null!;

    private uint lastJob = uint.MaxValue;
    private DateTime lastSortCommand = DateTime.MinValue;

    public SortaKindaController() {
        SortingThreadController = new SortingThreadController();
        SystemConfig = new SystemConfig();
        SortController = new SortController();
        ModuleController = new ModuleController();
        CommandManager = new CommandManager(Service.PluginInterface, "sortakinda", "sorta");
        WindowManager = new WindowManager(Service.PluginInterface);

        CommandManager.RegisterCommand(new CommandHandler {
            Delegate = SortCommand,
            ActivationPath = "/sort",
        });
        
        WindowManager.AddWindow(new ConfigurationWindow(), false, true);
        
        if (Service.ClientState is { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0 }) {
            OnLogin();
        }
        
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

        ModuleController.Dispose();
        SortingThreadController.Dispose();
        CommandManager.Dispose();
    }

    private void OnLogin() {
        if (!Service.ClientState.IsLoggedInNotPvP()) return;

        SystemConfig = new SystemConfig();
        SystemConfig = LoadConfig();
        SystemConfig.UpdateCharacterData(Service.ClientState);
        SaveConfig();

        SortController.Load();
        ModuleController.Load();

        if (SystemConfig.SortOnLogin) ModuleController.Sort();
    }

    private void OnLogout() {
        ModuleController.Unload();
        lastJob = uint.MaxValue;
    }

    private void OnUpdate(IFramework framework) {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;
        if (Service.ClientState is not { LocalPlayer.ClassJob.Id: var classJobId }) return;
        
        // Don't update modules if the Retainer transfer window is open
        if (Service.GameGui.GetAddonByName("RetainerItemTransferProgress") != nint.Zero) return;
        
        ModuleController.Update();

        // Prevent sorting on load, we have a different option for that
        if (lastJob is uint.MaxValue) lastJob = classJobId;

        if (SystemConfig.SortOnJobChange && lastJob != classJobId) {
            ModuleController.Sort();
            lastJob = classJobId;
        }
        
        SortingThreadController.Update();
    }

    private void OnZoneChange(ushort e) {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;

        if (SystemConfig.SortOnZoneChange) ModuleController.Sort();
    }

    private void OnInventoryChanged(IReadOnlyCollection<InventoryEventArgs> events) => ModuleController.InventoryChanged(events);

    private void SortCommand(params string[] args) {
        var timeSinceLastSort = DateTime.UtcNow - lastSortCommand;
        
        if (timeSinceLastSort.TotalSeconds > 10) {
            ModuleController.Sort();
            lastSortCommand = DateTime.UtcNow;
        }
        else {
            Service.ChatGui.PrintError($"Attempted to sort too soon after last sort. Try again in {10 - timeSinceLastSort.Seconds} seconds.");
        }
    }
    
    public static void DrawConfig() {
        var settingsChanged = ImGui.Checkbox("Sort on Item Added", ref SystemConfig.SortOnItemAdded);
        settingsChanged |= ImGui.Checkbox("Sort on Item Removed", ref SystemConfig.SortOnItemRemoved);
        settingsChanged |= ImGui.Checkbox("Sort on Item Changed", ref SystemConfig.SortOnItemChanged);
        settingsChanged |= ImGui.Checkbox("Sort on Item Moved", ref SystemConfig.SortOnItemMoved);
        settingsChanged |= ImGui.Checkbox("Sort on Item Merged", ref SystemConfig.SortOnItemMerged);
        settingsChanged |= ImGui.Checkbox("Sort on Item Split", ref SystemConfig.SortOnItemSplit);
        settingsChanged |= ImGui.Checkbox("Sort on Zone Change", ref SystemConfig.SortOnZoneChange);
        settingsChanged |= ImGui.Checkbox("Sort on Job Change", ref SystemConfig.SortOnJobChange);
        settingsChanged |= ImGui.Checkbox("Sort on Login", ref SystemConfig.SortOnLogin);
        settingsChanged |= ImGui.Checkbox("Reorder Unsorted Items", ref SystemConfig.ReorderUnsortedItems);

        if (settingsChanged) {
            SaveConfig();
        }
    }

    private static SystemConfig LoadConfig() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "System.config.json", () => new SystemConfig());

    private static void SaveConfig()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "System.config.json", SystemConfig);
}