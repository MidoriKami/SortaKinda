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
    public static RuleConfigController RuleConfigController = null!;
    public static SystemConfig SystemConfig = null!;
    
    private uint lastJob = uint.MaxValue;
    
    public SortaKindaController()
    {
        SystemConfig = new SystemConfig();
        RuleConfigController = new RuleConfigController();
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
        SortController.Dispose();
    }
    
    private void OnLogin(object? sender, EventArgs e)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: {  Name.TextValue: var playerName, HomeWorld.GameData.InternalName.RawString: var worldName }, LocalContentId: not 0, IsPvP: false }) return;

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

        SortController.Unload();
        ModuleController.Unload();

        lastJob = uint.MaxValue;
    }
    
    private void OnUpdate(Framework framework)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer.ClassJob.Id: var classJobId, LocalContentId: not 0, IsPvP: false }) return;

        SortController.Update();
        ModuleController.Update();

        // Prevent sorting on load, we have a different option for that
        if (lastJob is uint.MaxValue) lastJob = classJobId; 
        
        if (SystemConfig.SortOnJobChange && lastJob != classJobId)
        {
            ModuleController.Sort();
            lastJob = classJobId;
        }
    }
    
    private void OnZoneChange(object? sender, ushort e)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;
        
        if (SystemConfig.SortOnZoneChange) ModuleController.Sort();
    }
    
    public void DrawConfig() => DrawableAttribute.DrawAttributes(SystemConfig, SaveConfig);
 
    private SystemConfig LoadConfig() => CharacterFileController.LoadFile<SystemConfig>("System.config.json", SystemConfig);

    public void SaveConfig() => CharacterFileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig);
}