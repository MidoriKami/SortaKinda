using System;
using Dalamud.Game;

namespace SortaKinda.System;

public class SortaKindaController : IDisposable
{
    public static ModuleController ModuleController = null!;
    public static SortController SortController = null!;
    public static RuleConfigController RuleConfigController = null!;
    
    public SortaKindaController()
    {
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
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;

        SortController.Load();
        ModuleController.Load();
    }
    
    private void OnLogout(object? sender, EventArgs e)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;

        SortController.Unload();
        ModuleController.Unload();
    }
    
    private void OnUpdate(Framework framework)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;

        SortController.Update();
        ModuleController.Update();
    }
    
    private void OnZoneChange(object? sender, ushort e)
    {
        if (Service.ClientState is not { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0, IsPvP: false }) return;
        
        
    }
}