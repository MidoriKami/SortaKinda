using System.ComponentModel;

namespace SortaKinda.Data.Enums;

public enum SortOrderMode {
    [Description("Alphabetical")] 
    Alphabetically,
    
    [Description("ItemLevel")] 
    ItemLevel,

    [Description("Rarity")] 
    Rarity,

    [Description("SellPrice")] 
    SellPrice,

    [Description("ItemId")] 
    ItemId,
    
    [Description("ItemType")]
    ItemType,
}