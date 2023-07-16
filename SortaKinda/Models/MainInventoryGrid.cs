using System;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Abstracts;

namespace SortaKinda.Models;

public unsafe class MainInventoryGrid : InventoryGrid
{
    public MainInventoryGrid(InventoryType type) : base(type)
    {
        
    }
}