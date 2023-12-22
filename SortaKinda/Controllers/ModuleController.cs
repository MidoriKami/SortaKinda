using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Inventory;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Interfaces;
using SortaKinda.Models.Enums;
using SortaKinda.System.Modules;

namespace SortaKinda.System;

public class ModuleController : IDisposable {
    private readonly IEnumerable<IModule> modules = new List<IModule> {
        new MainInventoryModule(),
        new ArmoryInventoryModule()
    };

    public void Dispose() {
        Unload();

        foreach (var module in modules.OfType<IDisposable>()) {
            module.Dispose();
        }
    }

    public void Load() {
        foreach (var module in modules) {
            module.LoadModule();
        }
    }

    public void Unload() {
        foreach (var module in modules) {
            module.UnloadModule();
        }
    }

    public void Update() {
        foreach (var module in modules) {
            module.UpdateModule();
        }
    }

    public void Sort() {
        foreach (var module in modules) {
            module.SortModule();
        }
    }

    public void InventoryChanged(IReadOnlyCollection<InventoryEventArgs> events) {
        if (!Service.ClientState.IsLoggedIn) return;
        
        foreach (var module in modules) {
            var inventoryTypes = events
                .Where(itemEvent => itemEvent.Type switch {
                    GameInventoryEvent.Added when SortaKindaController.SystemConfig.SortOnItemAdded => true,
                    GameInventoryEvent.Removed when SortaKindaController.SystemConfig.SortOnItemRemoved => true,
                    GameInventoryEvent.Changed when SortaKindaController.SystemConfig.SortOnItemChanged => true,
                    GameInventoryEvent.Moved when SortaKindaController.SystemConfig.SortOnItemMoved => true,
                    GameInventoryEvent.Split when SortaKindaController.SystemConfig.SortOnItemSplit => true,
                    GameInventoryEvent.Merged when SortaKindaController.SystemConfig.SortOnItemMerged => true,
                    _ => false
                })
                .Select(itemEvent => (InventoryType) itemEvent.Item.ContainerType)
                .Where(inventoryType => module.InventoryTypes.Contains(inventoryType))
                .Distinct()
                .ToArray();

            if (inventoryTypes.Any()) {
                module.InventoryChanged(inventoryTypes);
            }
        }
    }

    public void DrawModule(ModuleName module) {
        modules.FirstOrDefault(drawableModule => drawableModule.ModuleName == module)?.Draw();
    }
}