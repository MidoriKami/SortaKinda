using System.Diagnostics.CodeAnalysis;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.GeneratedSheets;

namespace SortaKinda.Interfaces;

public unsafe interface IInventorySlot
{
    [MemberNotNullWhen(true, "Item")] bool HasItem { get; }

    Item? Item { get; }
    ItemOrderModuleSorterItemEntry* ItemOrderEntry { get; }
    int Slot { get; }
    ISortingRule Rule { get; }

    void OnLeftClick();
    void OnRightClick();
    void OnDragCollision();
    void OnHover();
}