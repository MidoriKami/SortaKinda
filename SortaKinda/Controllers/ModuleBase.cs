using Dalamud.Logging;
using KamiLib.Utilities;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Enum;

namespace SortaKinda.System;

public abstract class ModuleBase : IModule
{
    protected bool IsLoaded { get; set; }
    
    public abstract ModuleName ModuleName { get; protected set; }
    protected abstract IModuleConfig ModuleConfig { get; set; }
    protected abstract void Load();
    public abstract void Draw();
    protected abstract void Update();
    protected abstract void Sort();
    public virtual void Dispose() { }
    
    public void LoadModule()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");

        ModuleConfig = LoadConfig();
        Load();
        IsLoaded = true;
        
        SaveConfig();
    }
    
    public void UnloadModule()
    {
        IsLoaded = false;
    }
    
    public void UpdateModule()
    {
        if (!IsLoaded) return;
        
        var needsSaving = false;
        foreach (var inventory in ModuleConfig.InventoryConfigs)
        {
            foreach (var slot in inventory.SlotConfigs)
            {
                if (slot.Dirty)
                {
                    needsSaving = true;
                    slot.Dirty = false;
                }
            }
        }

        if (needsSaving) SaveConfig();

        Update();
    }
    
    public void SortModule()
    {
        if (!IsLoaded) return;
        
    }

    private IModuleConfig LoadConfig() => CharacterFileController.LoadFile<IModuleConfig>($"{ModuleName}.config.json", ModuleConfig);
    private void SaveConfig() => CharacterFileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);
}