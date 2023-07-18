using System;
using DailyDuty.System;
using Dalamud.Logging;
using KamiLib.Commands;
using SortaKinda.Models.Enum;

namespace SortaKinda.Abstracts;

public abstract class InventoryModuleBase : IDisposable
{
    public abstract ModuleName ModuleName { get; protected set; }
    public abstract IModuleConfig ModuleConfig { get; set; }

    public virtual void Dispose() { }

    public abstract void DrawInventoryGrid();
    protected abstract void LoadModule();
    public abstract void PerformSort();

    public void Load()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");
        
        ModuleConfig = LoadConfig();
        
        LoadModule();
        
        CommandController.RegisterSingleTierCommand(DoCommand, new SingleTierCommandHandler("test", "sort"));
    }

    public void Unload()
    {
        CommandController.UnregisterCommands(this);
    }
    
    private void DoCommand()
    {
        PerformSort();
    }

    private IModuleConfig LoadConfig() => FileController.LoadFile<IModuleConfig>($"{ModuleName}.config.json", ModuleConfig);
    public void SaveConfig() => FileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);
}