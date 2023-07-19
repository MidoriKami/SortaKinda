using System;
using DailyDuty.System;
using Dalamud.Game;
using KamiLib.AutomaticUserInterface;
using SortaKinda.Models;

namespace SortaKinda.System;

public class SortaKindaSystem : IDisposable
{
    public static ModuleController ModuleController = null!;
    public static SortController SortController = null!;
    public static SystemConfig SystemConfig = null!;
    public static WindowController WindowController = null!;

    private uint lastJob;
    
    public SortaKindaSystem()
    {
        SystemConfig = new SystemConfig();
        SystemConfig = LoadConfig();
        
        SortController = new SortController();
        ModuleController = new ModuleController();
        WindowController = new WindowController();

        if (Service.ClientState.IsLoggedIn)
        {
            OnLogin(this, EventArgs.Empty);
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.ClientState.TerritoryChanged += OnZoneChange;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;

        ModuleController.Update();

        var currentJob = Service.ClientState.LocalPlayer?.ClassJob.Id ?? 0;
        if (currentJob != lastJob && SystemConfig.SortOnJobChange)
        {
            ModuleController.SortAll();
        }
        lastJob = currentJob;
    }
    
    private void OnLogin(object? sender, EventArgs e)
    {
        ModuleController.Load();
        SortController.Load();
        
        if(SystemConfig.SortOnLogin) ModuleController.SortAll();
    }
    
    private void OnLogout(object? sender, EventArgs e)
    {
        ModuleController.Unload();
        SortController.Unload();
    }
    
    private void OnZoneChange(object? sender, ushort e)
    {
        if(SystemConfig.SortOnZoneChange) ModuleController.SortAll();
    }
    
    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.ClientState.TerritoryChanged += OnZoneChange;

        SortController.Dispose();
        ModuleController.Dispose();
        WindowController.Dispose();
    }

    public void DrawConfig() => DrawableAttribute.DrawAttributes(SystemConfig, SaveConfig);
    private SystemConfig LoadConfig() => FileController.LoadFile<SystemConfig>("System.config.json", SystemConfig);
    public void SaveConfig() => FileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig);
}