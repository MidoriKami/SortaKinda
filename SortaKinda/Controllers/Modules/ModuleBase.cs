using System;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.FileIO;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Models.Enums;

namespace SortaKinda.System;

public abstract class ModuleBase : IModule {
    private bool IsLoaded { get; set; }
    private float lastScale;
    
    protected abstract IModuleConfig ModuleConfig { get; set; }

    private IModuleConfig DefaultConfig => ModuleName switch {
        ModuleName.MainInventory => new MainInventoryConfig(),
        ModuleName.ArmoryInventory => new ArmoryConfig(),
        _ => throw new ArgumentOutOfRangeException()
    };

    public abstract ModuleName ModuleName { get; }
    
    public abstract void Draw();
    protected abstract void LoadViews();

    public virtual void Dispose() { }

    public void LoadModule() {
        Service.Log.Debug($"[{ModuleName}] Loading Module");

        ModuleConfig = DefaultConfig;
        ModuleConfig = LoadConfig();
        Load();
        LoadViews();
        IsLoaded = true;

        SaveConfig();

        SortaKindaController.InventoryScanner.InventoryChanged += InventoryChanged;
    }

    public void UnloadModule() {
        SortaKindaController.InventoryScanner.InventoryChanged -= InventoryChanged;
        
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

        Sort();
    }

    protected virtual void Load() { }

    protected abstract void InventoryChanged(InventoryType type);
    
    protected abstract void Sort();

    private IModuleConfig LoadConfig() => CharacterFileController.LoadFile<IModuleConfig>($"{ModuleName}.config.json", ModuleConfig);

    private void SaveConfig() => CharacterFileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);
}