using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Models.Enums;

namespace SortaKinda.Interfaces;

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