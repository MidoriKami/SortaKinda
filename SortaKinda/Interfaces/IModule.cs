using System;
using SortaKinda.Models.Enums;

namespace SortaKinda.Interfaces;

public interface IModule : IDisposable {
    ModuleName ModuleName { get; }

    void LoadModule();
    void UnloadModule();
    void UpdateModule();
    void SortModule();
    void Draw();
}