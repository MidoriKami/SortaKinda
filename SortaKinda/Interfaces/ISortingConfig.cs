using KamiLib.AutomaticUserInterface;

namespace SortaKinda.Models;

[Category("SortingOptions")]
public interface ISortingConfig
{
    [BoolConfig("SortOnInventoryChange")] public bool SortOnInventoryChange { get; set; }

    [BoolConfig("SortOnZoneChange")] public bool SortOnZoneChange { get; set; }

    [BoolConfig("SortOnJobChange")] public bool SortOnJobChange { get; set; }

    [BoolConfig("SortOnLogin")] public bool SortOnLogin { get; set; }
}