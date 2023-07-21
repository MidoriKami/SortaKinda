using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Models;

namespace SortaKinda.Interfaces;

public interface IInventorySlot
{
    Item? LuminaData { get; }
    Pointer<ItemOrderModuleSorterItemEntry> ItemOrderData { get; }
    bool HasItem { get; }
    ISortingRule Rule { get; }
    void Draw(Vector2 drawPosition, Vector2 size);
}