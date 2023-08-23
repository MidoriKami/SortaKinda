using System;
using Dalamud.Logging;
using KamiLib.Utilities;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Enums;

namespace SortaKinda.System;

public abstract class ModuleBase : IModule
{
    private bool IsLoaded { get; set; }
    
    protected abstract IModuleConfig ModuleConfig { get; set; }

    private IModuleConfig DefaultConfig => ModuleName switch
    {
        ModuleName.MainInventory => new MainInventoryConfig(),
        ModuleName.ArmoryInventory => new ArmoryConfig(),
        _ => throw new ArgumentOutOfRangeException()
    };

    public abstract ModuleName ModuleName { get; }
    
    public abstract void Draw();
    
    public virtual void Dispose()
    {
    }

    public void LoadModule()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");

        ModuleConfig = DefaultConfig;
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

        // Don't update modules if the Retainer transfer window is open
        if (Service.GameGui.GetAddonByName("RetainerItemTransferProgress") != nint.Zero) return;
        Update();
    }

    public void SortModule()
    {
        if (!IsLoaded) return;

        Sort();
    }
    
    protected abstract void Load();
    
    protected abstract void Update();
    
    protected abstract void Sort();

    private IModuleConfig LoadConfig()
    {
        return CharacterFileController.LoadFile<IModuleConfig>($"{ModuleName}.config.json", ModuleConfig);
    }
    
    private void SaveConfig()
    {
        CharacterFileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);
    }
}