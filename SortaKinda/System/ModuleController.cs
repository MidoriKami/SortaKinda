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

    public void Dispose()
    {
        foreach (var module in modules)
        {
            module.Dispose();
        }
    }

    public InventoryModuleBase GetModule(ModuleName name)
    {
        return modules.First(eachModule => eachModule.ModuleName == name);
    }

    public void Load()
    {
        foreach (var module in modules)
        {
            module.Load();
        }
    }

    public void SortAll()
    {
        foreach (var module in modules)
        {
            module.SortAll();
        }
    }

    public void Update()
    {
        foreach (var module in modules)
        {
            module.Update();
        }
    }
}