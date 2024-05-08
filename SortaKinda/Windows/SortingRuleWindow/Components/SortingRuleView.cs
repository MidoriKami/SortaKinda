using KamiLib.TabBar;
using SortaKinda.Configuration;

namespace SortaKinda.Windows.SortingRuleWindow.Components;

public class SortingRuleView(SortingRule rule) {
    private readonly TabBar tabBar = new("SortingRuleTabBar", [
        new ItemNameFilterTab(rule),
        new ItemTypeFilterTab(rule),
        new OtherFiltersTab(rule),
        new ToggleFiltersTab(rule),
        new SortOrderTab(rule)
    ], false);

    public void Draw() => tabBar.Draw();
}