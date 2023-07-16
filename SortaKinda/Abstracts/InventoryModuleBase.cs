using System;
using DailyDuty.System;
using Dalamud.Logging;
using SortaKinda.Models.Enum;

namespace SortaKinda.Abstracts;

public abstract class InventoryModuleBase : IDisposable
{
    public abstract ModuleName ModuleName { get; protected set; }
    public abstract IInventoryConfig ModuleConfig { get; protected set; }

    public virtual void Dispose() { }

    public abstract void DrawInventoryGrid();

    public void Load()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");
        
        ModuleConfig = LoadConfig();
    }

    public void Unload()
    {
        
    }
    
    private IInventoryConfig LoadConfig() => FileController.LoadFile<IInventoryConfig>($"{ModuleName}.config.json", ModuleConfig);
    public void SaveConfig() => FileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);
}