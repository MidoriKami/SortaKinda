using Dalamud.Logging;
using KamiLib.Utilities;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Enum;

namespace SortaKinda.System;

public abstract class ModuleBase : IModule
{
    protected bool IsLoaded { get; set; }
    protected bool NeedsSaving { get; set; }
    
    public abstract ModuleName ModuleName { get; protected set; }
    protected abstract IModuleConfig ModuleConfig { get; set; }
    protected abstract void Initialize();
    public abstract void Draw();
    protected abstract void InternalUpdate();
    protected abstract void DoSort();
    public virtual void Dispose() { }
    
    public void Load()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");

        ModuleConfig = LoadConfig();
        Initialize();
        IsLoaded = true;
        
        SaveConfig();
    }
    
    public void Unload()
    {
        IsLoaded = false;
    }
    
    public void Update()
    {
        if (!IsLoaded) return;
        if (NeedsSaving) SaveConfig();
    }
    
    public void Sort()
    {
        if (!IsLoaded) return;
        
    }

    private IModuleConfig LoadConfig() => CharacterFileController.LoadFile<IModuleConfig>($"{ModuleName}.config.json", ModuleConfig);
    private void SaveConfig() => CharacterFileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);
}