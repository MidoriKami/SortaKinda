using KamiLib.TabBar;
using SortaKinda.Models;
using SortaKinda.Views.Tabs;

namespace SortaKinda.Views.SortControllerViews;

public class SortingRuleView(SortingRule rule) {
    private readonly TabBar tabBar = new("SortingRuleTabBar", [
        new ItemNameFilterTab(rule),
        new ItemTypeFilterTab(rule),
        new OtherFiltersTab(rule),
        new ToggleFiltersTab(rule),
        new SortOrderTab(rule)
    ]);

    public void Draw() => tabBar.Draw();
}