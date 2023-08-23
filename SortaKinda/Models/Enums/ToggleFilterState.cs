using KamiLib.AutomaticUserInterface;

namespace SortaKinda.Models.Enums;

public enum ToggleFilterState
{
    [EnumLabel("Ignored")]
    Ignored,
    
    [EnumLabel("Allow")]
    Allow,
    
    [EnumLabel("Disallow")]
    Disallow,
}