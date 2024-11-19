using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Inventory;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Modules;

namespace SortaKinda.Controllers;

public class ModuleController : IDisposable {
    private readonly List<IModule> modules = [
        new MainInventoryModule(),
        new ArmoryInventoryModule(),
    ];

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

    public IModule GetModule<T>() where T : IModule
        => modules.OfType<T>().First();

    public void InventoryChanged(IReadOnlyCollection<InventoryEventArgs> events) {
        if (!Service.ClientState.IsLoggedIn) return;
        
        foreach (var module in modules) {
            var inventoryTypes = new HashSet<InventoryType>();

            foreach (var itemEvent in events) {
                if (!IsEventAllowed(itemEvent)) continue;

                AddChangedInventories(itemEvent, inventoryTypes);

                inventoryTypes.RemoveWhere(type => !module.InventoryTypes.Contains(type));
            }

            if (inventoryTypes.Count != 0) {
                module.InventoryChanged(inventoryTypes.ToArray());
            }
        }
    }

    private static void AddChangedInventories(InventoryEventArgs itemEvent, ICollection<InventoryType> inventoryTypes) {
        switch (itemEvent) {
            case null:
                break;

            case InventoryItemAddedArgs itemAdded:
                inventoryTypes.Add((InventoryType) itemAdded.Inventory);
                break;
            
            case InventoryItemRemovedArgs itemRemoved:
                inventoryTypes.Add((InventoryType) itemRemoved.Inventory);
                break;

            case InventoryItemChangedArgs itemChanged:
                inventoryTypes.Add((InventoryType) itemChanged.Inventory);
                break;

            case InventoryItemMergedArgs itemMerged:
                inventoryTypes.Add((InventoryType) itemMerged.SourceInventory);
                inventoryTypes.Add((InventoryType) itemMerged.TargetInventory);
                break;

            case InventoryItemMovedArgs itemMoved:
                inventoryTypes.Add((InventoryType) itemMoved.SourceInventory);
                inventoryTypes.Add((InventoryType) itemMoved.TargetInventory);
                break;

            case InventoryItemSplitArgs itemSplit:
                inventoryTypes.Add((InventoryType) itemSplit.SourceInventory);
                inventoryTypes.Add((InventoryType) itemSplit.TargetInventory);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(itemEvent), itemEvent, null);
        }
    }

    public void DrawModule(ModuleName module) {
        modules.FirstOrDefault(drawableModule => drawableModule.ModuleName == module)?.Draw();
    }

    private static bool IsEventAllowed(InventoryEventArgs argument) => argument.Type switch {
        GameInventoryEvent.Added when System.SystemConfig.SortOnItemAdded => true,
        GameInventoryEvent.Removed when System.SystemConfig.SortOnItemRemoved => true,
        GameInventoryEvent.Changed when System.SystemConfig.SortOnItemChanged => true,
        GameInventoryEvent.Moved when System.SystemConfig.SortOnItemMoved => true,
        GameInventoryEvent.Split when System.SystemConfig.SortOnItemSplit => true,
        GameInventoryEvent.Merged when System.SystemConfig.SortOnItemMerged => true,
        _ => false,
    };
}