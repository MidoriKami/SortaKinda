namespace SortaKinda.Models;


public class SystemConfig : ISortingConfig, IOrderingConfig
{
    public bool SortOnInventoryChange { get; set; } = true;
    public bool SortOnZoneChange { get; set; } = true;
    public bool SortOnJobChange { get; set; } = true;
    public bool SortOnLogin { get; set; } = true;

    public bool FillFromBottom { get; set; } = false;
}