namespace SortaKinda.Models.Configuration;

public class SystemConfig : ISortingConfig {
    public string CharacterName { get; set; } = "Unknown Character Name";
    public string HomeWorld { get; set; } = "Unknown HomeWorld";
    public bool SortOnItemAdded { get; set; } = true;
    public bool SortOnItemRemoved { get; set; } = true;
    public bool SortOnItemChanged { get; set; } = false;
    public bool SortOnItemMoved { get; set; } = false;
    public bool SortOnItemMerged { get; set; } = false;
    public bool SortOnItemSplit { get; set; } = false;
    public bool SortOnZoneChange { get; set; } = true;
    public bool SortOnJobChange { get; set; } = true;
    public bool SortOnLogin { get; set; } = true;
}