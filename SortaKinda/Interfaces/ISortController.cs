using SortaKinda.Models.Configuration;

namespace SortaKinda.Interfaces;

public interface ISortController
{
    SortingRuleConfig RuleConfig { get; set; }
}