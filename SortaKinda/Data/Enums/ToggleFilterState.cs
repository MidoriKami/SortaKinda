using System.ComponentModel;

namespace SortaKinda.Models.Enums;

public enum ToggleFilterState {
    [Description("Ignored")]
    Ignored,
    
    [Description("Allow")]
    Allow,
    
    [Description("Disallow")]
    Disallow,
}