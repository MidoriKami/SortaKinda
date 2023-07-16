using System;
using System.Collections.Generic;
using System.Linq;
using KamiLib.Utilities;
using SortaKinda.Abstracts;
using SortaKinda.Models.Enum;

namespace SortaKinda.System;

public class ModuleController : IDisposable
{
    private readonly IEnumerable<InventoryModuleBase> modules;

    public ModuleController()
    {
        modules = Reflection.ActivateOfType<InventoryModuleBase>().ToList();
    }

    public InventoryModuleBase GetModule(ModuleName name)
        => modules.First(eachModule => eachModule.ModuleName == name);

    public void Load()
    {
        foreach (var module in modules)
        {
            module.Load();
        }
    }

    public void Unload()
    {
        foreach (var module in modules)
        {
            module.Unload();
        }
    }
    
    public void Dispose()
    {
        foreach (var module in modules)
        {
            module.Dispose();
        }
    }
}