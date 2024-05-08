using SortaKinda.Configuration;
using SortaKinda.Windows.SortingRuleWindow.Components;

namespace SortaKinda.Windows.SortingRuleWindow;

public class ToggleFiltersTab(SortingRule rule) : IOneColumnRuleConfigurationTab {
    public string Name => "Property Filters";
    
    public bool Disabled => false;
    
    public SortingRule SortingRule { get; } = rule;
    
    public string FirstLabel => "Property Filters";
    
    public void DrawContents() {
        SortingRule.UntradableFilter.DrawConfig();
        SortingRule.UniqueFilter.DrawConfig();
        SortingRule.DyeableFilter.DrawConfig();
        SortingRule.CollectableFilter.DrawConfig();
        SortingRule.RepairableFilter.DrawConfig();
    }
}