using KamiLib.AutomaticUserInterface;

namespace SortaKinda.Models;

[Category("SortingOptions")]
public interface ISortingConfig {
    [BoolConfig("SortOnItemAdded")]
    public bool SortOnItemAdded { get; set; }
    
    [BoolConfig("SortOnItemRemoved")]
    public bool SortOnItemRemoved { get; set; }
    
    [BoolConfig("SortOnItemChanged")]
    public bool SortOnItemChanged { get; set; }
    
    [BoolConfig("SortOnItemMoved", "SortOnItemMovedWarning")]
    public bool SortOnItemMoved { get; set; }
    
    [BoolConfig("SortOnItemMerged")]
    public bool SortOnItemMerged { get; set; }
    
    [BoolConfig("SortOnItemSplit")]
    public bool SortOnItemSplit { get; set; }
    
    [BoolConfig("SortOnZoneChange")] 
    public bool SortOnZoneChange { get; set; }

    [BoolConfig("SortOnJobChange")] 
    public bool SortOnJobChange { get; set; }

    [BoolConfig("SortOnLogin")] 
    public bool SortOnLogin { get; set; }
}