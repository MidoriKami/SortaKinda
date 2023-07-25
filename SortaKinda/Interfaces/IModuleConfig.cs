using System.Collections.Generic;

namespace SortaKinda.Models.Configuration;

public interface IModuleConfig
{
    List<InventoryConfig> InventoryConfigs { get; set; }
}