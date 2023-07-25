using System;
using SortaKinda.System;

namespace SortaKinda.Models.Configuration;

public class SlotConfig
{
    [NonSerialized] public bool Dirty;

    public string RuleId { get; set; } = SortController.DefaultId;
}