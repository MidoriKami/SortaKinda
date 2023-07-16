using System;
using Dalamud.Game;

namespace SortaKinda.System;

public class SortaKindaSystem : IDisposable
{
    public static ModuleController ModuleController = null!;
    
    public SortaKindaSystem()
    {
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

        ModuleController.Dispose();
    }
}