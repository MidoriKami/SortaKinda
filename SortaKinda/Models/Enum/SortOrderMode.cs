using KamiLib.AutomaticUserInterface;

namespace SortaKinda.Models.Enum;

public enum SortOrderMode
{
    [EnumLabel("Alphabetical")]
    Alphabetically,
    
    [EnumLabel("ItemLevel")]
    ItemLevel,
    
    [EnumLabel("Rarity")]
    Rarity,
    
    [EnumLabel("SellPrice")]
    SellPrice,
    
    [EnumLabel("ItemId")]
    ItemId,
}