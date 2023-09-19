using System;
using Dalamud.Plugin.Services;
using KamiLib.AutomaticUserInterface;
using KamiLib.Utilities;
using SortaKinda.Models.Configuration;

namespace SortaKinda.System;

public class SortaKindaController : IDisposable
{
    public static ModuleController ModuleController = null!;
    public static SortController SortController = null!;
    public static SystemConfig SystemConfig = null!;
    public static SortingThreadController SortingThreadController = null!;
    public static InventoryScanner InventoryScanner = null!;

    private uint lastJob = uint.MaxValue;

    public SortaKindaController()
    {
        SortingThreadController = new SortingThreadController();
        InventoryScanner = new InventoryScanner();
        SystemConfig = new SystemConfig();
        SortController = new SortController();
        ModuleController = new ModuleController();

        if (Service.ClientState is { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0 })
        {
            OnLogin();
        }

        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.Framework.Update += OnUpdate;
        Service.ClientState.TerritoryChanged += OnZoneChange;
    }

    public void Dispose()
    {
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.Framework.Update -= OnUpdate;
        Service.ClientState.TerritoryChanged -= OnZoneChange;

        ModuleController.Dispose();
        SortingThreadController.Dispose();
    }

    private void OnLogin()
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;
        if (Service.ClientState is not { LocalPlayer: { Name.TextValue: var playerName, HomeWorld.GameData.InternalName.RawString: var worldName } }) return;

        SystemConfig = new SystemConfig();
        SystemConfig = LoadConfig();
        SystemConfig.CharacterName = playerName;
        SystemConfig.HomeWorld = worldName;
        SaveConfig();

        SortController.Load();
        ModuleController.Load();

        if (SystemConfig.SortOnLogin) ModuleController.Sort();
    }

    private void OnLogout()
    {
        ModuleController.Unload();

        lastJob = uint.MaxValue;
    }

    private void OnUpdate(IFramework framework)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;
        if (Service.ClientState is not { LocalPlayer.ClassJob.Id: var classJobId }) return;
        
        // Don't update modules if the Retainer transfer window is open
        if (Service.GameGui.GetAddonByName("RetainerItemTransferProgress") != nint.Zero) return;
        
        InventoryScanner.Update();
        ModuleController.Update();

        // Prevent sorting on load, we have a different option for that
        if (lastJob is uint.MaxValue) lastJob = classJobId;

        if (SystemConfig.SortOnJobChange && lastJob != classJobId)
        {
            ModuleController.Sort();
            lastJob = classJobId;
        }
        
        SortingThreadController.Update();
    }

    private void OnZoneChange(ushort e)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;

        if (SystemConfig.SortOnZoneChange) ModuleController.Sort();
    }

    public static void DrawConfig()
    {
        DrawableAttribute.DrawAttributes(SystemConfig, SaveConfig);
    }
    
    private static SystemConfig LoadConfig() => CharacterFileController.LoadFile<SystemConfig>("System.config.json", SystemConfig);

    private static void SaveConfig() => CharacterFileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig);
}