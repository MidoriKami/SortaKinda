using System;
using DailyDuty.System;
using Dalamud.Game;
using SortaKinda.Models;

namespace SortaKinda.System;

public class SortaKindaSystem : IDisposable
{
    public static ModuleController ModuleController = null!;
    public static SortController SortController = null!;
    public static SystemConfig SystemConfig = null!;

    public SortaKindaSystem()
    {
        SystemConfig = new SystemConfig();
        SystemConfig = LoadConfig();
        
        SortController = new SortController();
        ModuleController = new ModuleController();

        if (Service.ClientState.IsLoggedIn)
        {
            OnLogin(this, EventArgs.Empty);
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
    }
    
    private void OnFrameworkUpdate(Framework framework)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;

    }
    
    private void OnLogin(object? sender, EventArgs e)
    {
        ModuleController.Load();
    }
    
    private void OnLogout(object? sender, EventArgs e)
    {
        ModuleController.Unload();
    }
    
    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;

        SortController.Dispose();
        ModuleController.Dispose();
    }
    
    private SystemConfig LoadConfig() => FileController.LoadFile<SystemConfig>("System.config.json", SystemConfig);
    public void SaveConfig() => FileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig);
}