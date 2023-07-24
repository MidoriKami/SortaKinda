using System;
using Dalamud.Game;

namespace SortaKinda.System;

public class SortaKindaController : IDisposable
{
    public ModuleController ModuleController;
    public SortController SortController;
    
    public SortaKindaController()
    {
        ModuleController = new ModuleController();
        SortController = new SortController();

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
    }
    
    private void OnLogin(object? sender, EventArgs e)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;
        
        ModuleController.Load();
        SortController.Load();
    }
    
    private void OnLogout(object? sender, EventArgs e)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;
        
        ModuleController.Unload();
        SortController.Unload();
    }
    
    private void OnUpdate(Framework framework)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;
        
        ModuleController.Update();
    }
    
    private void OnZoneChange(object? sender, ushort e)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;
        
        
    }
}