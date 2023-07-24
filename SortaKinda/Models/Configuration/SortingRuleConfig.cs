using System.Collections.Generic;
using System.Drawing;
using KamiLib.Utilities;
using SortaKinda.System;

namespace SortaKinda.Models.Configuration;

public class SortingRuleConfig
{
    public List<SortingRule> Rules { get; set; } = new()
    {
        new SortingRule
        {
            Color = KnownColor.White.AsVector4(),
            Id = SortController.DefaultId,
            Name = "Unsorted",
        }
    };
}