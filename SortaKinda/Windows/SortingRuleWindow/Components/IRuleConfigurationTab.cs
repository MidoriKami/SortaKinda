using KamiLib.TabBar;

namespace SortaKinda.Windows.SortingRuleWindow.Components;

public interface IRuleConfigurationTab : ITabItem {
    void ITabItem.Draw() 
        => DrawConfigurationTab();

    void DrawConfigurationTab();
}