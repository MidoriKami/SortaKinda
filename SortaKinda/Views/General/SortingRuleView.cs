using System.Collections.Generic;
using KamiLib.Interfaces;
using KamiLib.Windows;
using SortaKinda.Interfaces;
using SortaKinda.Views.Tabs;

namespace SortaKinda.Views.SortControllerViews;

public class SortingRuleView
{
    private readonly TabBar tabBar;

    public SortingRuleView(ISortingRule rule)
    {
        tabBar = new TabBar
        {
            Id = "SortingRuleTabBar",
            TabItems = new List<ITabItem>
            {
                new ItemNameFilterTab(rule),
                new ItemTypeFilterTab(rule),
                new OtherFiltersTab(rule),
                new SortOrderTab(rule),
            }
        };
    }

    public void Draw()
    {
        tabBar.Draw();
    }
}