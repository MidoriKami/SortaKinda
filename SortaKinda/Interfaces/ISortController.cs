using System.Collections.Generic;
using SortaKinda.Models;

namespace SortaKinda.Interfaces;

public interface ISortController {
    List<SortingRule> Rules { get; }

    void SortAllInventories();
    void SaveConfig();
}