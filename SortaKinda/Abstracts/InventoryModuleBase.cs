using System;
using DailyDuty.System;
using Dalamud.Logging;
using SortaKinda.Interfaces;
using SortaKinda.Models.Enum;

namespace SortaKinda.Abstracts;

public abstract class InventoryModuleBase : IDisposable
{
    public abstract ModuleName ModuleName { get; protected set; }
    public abstract IModuleConfig ModuleConfig { get; protected set; }

    public virtual void Dispose() { }

    public abstract void DrawInventoryGrid();
    protected abstract void LoadModule();
    protected abstract void PerformSort();
    public abstract void Update();
    
    protected bool IsLoaded { get; set; }

    public void Load()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");
        
        ModuleConfig = LoadConfig();
        LoadModule();

        IsLoaded = true;
    }

    public void Unload()
    {
        IsLoaded = false;
    }
    
    public virtual void SortAll()
    {
        if (IsLoaded) PerformSort();
    }

    private IModuleConfig LoadConfig()
    {
        return FileController.LoadFile<IModuleConfig>($"{ModuleName}.config.json", ModuleConfig);
    }

    public void SaveConfig()
    {
        FileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);
    }
}