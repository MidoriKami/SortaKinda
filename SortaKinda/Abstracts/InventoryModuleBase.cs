using System;
using DailyDuty.System;
using Dalamud.Logging;
using SortaKinda.Interfaces;
using SortaKinda.Models.Enum;

namespace SortaKinda.Abstracts;

public abstract class InventoryModuleBase : IDisposable
{
    public abstract ModuleName ModuleName { get; protected set; }
    public abstract IModuleConfig ModuleConfig { get; set; }

    public virtual void Dispose()
    {
    }

    public abstract void DrawInventoryGrid();
    protected abstract void LoadModule();
    public abstract void SortAll();
    public abstract void Update();

    public void Load()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");

        ModuleConfig = LoadConfig();
        var newConfig = ModuleConfig.Configurations is null;
        LoadModule();
        if (newConfig) SaveConfig();
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