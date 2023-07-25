using System.Collections.Generic;
using System.Numerics;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.Interfaces;

public interface ISortingRule : IComparer<IInventorySlot>
{
    Vector4 Color { get; set; }
    string Id { get; set; }
    string Name { get; set; }
    int Index { get; set; }
    
    HashSet<string> AllowedItemNames { get; set; }
    HashSet<uint> AllowedItemTypes { get; set; }
    HashSet<ItemRarity> AllowedItemRarities { get; set; }
    RangeFilter ItemLevelFilter { get; set; }
    RangeFilter VendorPriceFilter { get; set; }
    
    public SortOrderDirection Direction { get; set; }
    public FillMode FillMode { get; set; }
    public SortOrderMode SortMode { get; set; }
    
    
    void ShowTooltip();
    public bool IsItemSlotAllowed(IInventorySlot slot);
}