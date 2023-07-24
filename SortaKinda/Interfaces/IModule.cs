using System;
using SortaKinda.Models.Enum;

namespace SortaKinda.Interfaces;

public interface IModule : IDisposable
{
    ModuleName ModuleName { get; }
    
    void Load();
    void Unload();
    void Update();
    void Sort();
    void Draw();
}