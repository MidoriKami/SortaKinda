using System.Collections.Generic;
using System.Numerics;
using SortaKinda.Models;
using SortaKinda.Models.Enums;
using SortaKinda.Models.General;

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
    ToggleFilter UntradableFilter { get; }
    ToggleFilter UniqueFilter { get; }
    ToggleFilter CollectableFilter { get; }
    ToggleFilter DyeableFilter { get; }

    SortOrderDirection Direction { get; set; }
    FillMode FillMode { get; set; }
    SortOrderMode SortMode { get; set; }

    void ShowTooltip();
    bool IsItemSlotAllowed(IInventorySlot slot);
}