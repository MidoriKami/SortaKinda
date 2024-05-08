using System.Text.Json.Serialization;
using SortaKinda.System;

namespace SortaKinda.Models.Configuration;

public class SlotConfig {
    [JsonIgnore] public bool Dirty;

    public string RuleId { get; set; } = SortController.DefaultId;
}