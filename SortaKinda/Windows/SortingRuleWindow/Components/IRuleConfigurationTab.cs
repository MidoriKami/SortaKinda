using KamiLib.TabBar;

namespace SortaKinda.Interfaces;

public interface IRuleConfigurationTab : ITabItem {
    void ITabItem.Draw() 
        => DrawConfigurationTab();

    void DrawConfigurationTab();
}