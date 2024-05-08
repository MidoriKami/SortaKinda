using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.Classes;
using SortaKinda.Configuration;
using SortaKinda.Data;
using SortaKinda.Data.Enums;

namespace SortaKinda.Modules;

public interface IModule : IDisposable {
    ModuleName ModuleName { get; }
    
    IEnumerable<InventoryType> InventoryTypes { get; }

    void LoadModule();
    
    void UnloadModule();
    
    void UpdateModule();
    
    void SortModule();
    
    void Draw();
    
    void InventoryChanged(params InventoryType[] changedInventories);
}

public interface IModuleConfig {
    List<InventoryConfig> InventoryConfigs { get; set; }
}

public abstract class ModuleBase<T> : IModule where T : IModuleConfig, new() {
    private bool IsLoaded { get; set; }
    
    private float lastScale;
    
    protected abstract List<InventoryGrid> Inventories { get; set; }
    
    public IEnumerable<InventoryType> InventoryTypes 
        => Inventories.Select(inventory => inventory.Type);

    protected abstract T ModuleConfig { get; set; }

    public abstract ModuleName ModuleName { get; }

    public abstract void Draw();

    protected abstract void LoadViews();

    public virtual void Dispose() { }

    public void LoadModule() {
        Service.Log.Debug($"[{ModuleName}] Loading Module");

        ModuleConfig = new T();
        ModuleConfig = LoadConfig();
        Load();
        LoadViews();
        IsLoaded = true;

        SaveConfig();
    }

    public void UnloadModule() {
        IsLoaded = false;
    }

    public void UpdateModule() {
        if (!IsLoaded) return;

        var needsSaving = false;
        foreach (var inventory in ModuleConfig.InventoryConfigs) {
            foreach (var slot in inventory.SlotConfigs) {
                if (slot.Dirty) {
                    needsSaving = true;
                    slot.Dirty = false;
                }
            }
        }

        if (needsSaving) SaveConfig();

        if (!ImGuiHelpers.GlobalScale.Equals(lastScale)) {
            LoadViews();
        }
        lastScale = ImGuiHelpers.GlobalScale;
    }

    public void SortModule() {
        if (!IsLoaded) return;

        Sort(InventoryTypes.ToArray());
    }

    protected virtual void Load() { }

    public void InventoryChanged(params InventoryType[] changedInventories)
        => Sort(changedInventories);

    protected abstract void Sort(params InventoryType[] inventories);

    private T LoadConfig()
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, $"{ModuleName}.config.json", () => new T());

    private void SaveConfig() 
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, $"{ModuleName}.config.json", ModuleConfig);
}