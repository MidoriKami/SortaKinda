using System.ComponentModel;

namespace SortaKinda.Models.Enums;

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