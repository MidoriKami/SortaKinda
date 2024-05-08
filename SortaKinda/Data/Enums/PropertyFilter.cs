using System.ComponentModel;

namespace SortaKinda.Data.Enums;

public enum PropertyFilter {
    [Description("Untradable")]
    Untradable,
    
    [Description("Dyeable")]
    Dyeable,
    
    [Description("Unique")]
    Unique,
    
    [Description("Collectable")]
    Collectable,
    
    [Description("Repairable")]
    Repairable,
}