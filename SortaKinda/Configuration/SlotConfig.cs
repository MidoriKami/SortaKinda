using System.Text.Json.Serialization;
using SortaKinda.Controllers;

namespace SortaKinda.Configuration;

public class SlotConfig {
    [JsonIgnore] public bool Dirty;

    public string RuleId { get; set; } = SortController.DefaultId;
}