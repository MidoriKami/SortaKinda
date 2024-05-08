using System.ComponentModel;

namespace SortaKinda.Data.Enums;

public enum ToggleFilterState {
    [Description("Ignored")]
    Ignored,
    
    [Description("Allow")]
    Allow,
    
    [Description("Disallow")]
    Disallow,
}