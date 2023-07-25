using System.Collections.Generic;
using System.Numerics;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.Interfaces;

public interface ISortingRule : IComparer<IInventorySlot>
{
    Vector4 Color { get; set; }
    string Id { get; }
    string Name { get; }
    int Index { get; }

    HashSet<string> AllowedItemNames { get; }
    HashSet<uint> AllowedItemTypes { get; }
    HashSet<ItemRarity> AllowedItemRarities { get; }
    RangeFilter ItemLevelFilter { get; }
    RangeFilter VendorPriceFilter { get; }

    public SortOrderDirection Direction { get; set; }
    public FillMode FillMode { get; set; }
    public SortOrderMode SortMode { get; set; }

    void ShowTooltip();
    public bool IsItemSlotAllowed(IInventorySlot slot);
}