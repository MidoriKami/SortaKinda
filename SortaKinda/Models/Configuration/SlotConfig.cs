using System;
using SortaKinda.System;

namespace SortaKinda.Models.Configuration;

public class SlotConfig
{
    public string RuleId { get; set; } = SortController.DefaultId;

    [NonSerialized] 
    public bool Dirty;
}