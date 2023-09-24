using System.Collections.Generic;
using System.Drawing;
using Dalamud.Interface;
using SortaKinda.System;

namespace SortaKinda.Models.Configuration;

public class SortingRuleConfig
{
    public List<SortingRule> Rules { get; set; } = new()
    {
        new SortingRule
        {
            Color = KnownColor.White.Vector(),
            Id = SortController.DefaultId,
            Name = "Unsorted"
        }
    };
}