using System.Diagnostics.CodeAnalysis;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Models.Configuration;

namespace SortaKinda.Interfaces;

public unsafe interface IInventorySlot
{
    [MemberNotNullWhen(true, "Item")] 
    bool HasItem { get; }
    Item? Item { get; }
    ItemOrderModuleSorterItemEntry* ItemOrderEntry { get; }
    int Slot { get; init; }
    SlotConfig Config { get; init; }
    ISortingRule Rule { get; }

    void OnLeftClick();
    void OnRightClick();
    void OnDragCollision();
    void OnHover();
}