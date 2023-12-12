namespace SortaKinda.Models.Configuration;

public class SystemConfig : ISortingConfig {
    public string CharacterName { get; set; } = "Unknown Character Name";
    public string HomeWorld { get; set; } = "Unknown HomeWorld";
    public bool SortOnInventoryChange { get; set; } = true;
    public bool SortOnZoneChange { get; set; } = true;
    public bool SortOnJobChange { get; set; } = true;
    public bool SortOnLogin { get; set; } = true;
}