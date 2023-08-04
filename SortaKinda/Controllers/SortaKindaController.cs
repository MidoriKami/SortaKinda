using System;
using Dalamud.Game;
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

    private uint lastJob = uint.MaxValue;

    public SortaKindaController()
    {
        SortingThreadController = new SortingThreadController();
        SystemConfig = new SystemConfig();
        SortController = new SortController();
        ModuleController = new ModuleController();

        if (Service.ClientState is { IsLoggedIn: true, LocalPlayer: not null })
        {
            OnLogin(this, EventArgs.Empty);
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

    private void OnLogin(object? sender, EventArgs e)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: { Name.TextValue: var playerName, HomeWorld.GameData.InternalName.RawString: var worldName }, LocalContentId: not 0, IsPvP: false }) return;

        SystemConfig = new SystemConfig();
        SystemConfig = LoadConfig();
        SystemConfig.CharacterName = playerName;
        SystemConfig.HomeWorld = worldName;
        SaveConfig();

        SortController.Load();
        ModuleController.Load();

        if (SystemConfig.SortOnLogin && sender != this) ModuleController.Sort();
    }

    private void OnLogout(object? sender, EventArgs e)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;

        ModuleController.Unload();

        lastJob = uint.MaxValue;
    }

    private void OnUpdate(Framework framework)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer.ClassJob.Id: var classJobId, LocalContentId: not 0, IsPvP: false }) return;

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

    private void OnZoneChange(object? sender, ushort e)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;

        if (SystemConfig.SortOnZoneChange) ModuleController.Sort();
    }

    public static void DrawConfig()
    {
        DrawableAttribute.DrawAttributes(SystemConfig, SaveConfig);
    }
    
    private static SystemConfig LoadConfig()
    {
        return CharacterFileController.LoadFile<SystemConfig>("System.config.json", SystemConfig);
    }
    
    private static void SaveConfig()
    {
        CharacterFileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig);
    }
}