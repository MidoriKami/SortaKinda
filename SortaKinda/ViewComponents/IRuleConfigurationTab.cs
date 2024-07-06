using KamiLib.Classes;

namespace SortaKinda.ViewComponents;

public interface IRuleConfigurationTab : ITabItem {
    void ITabItem.Draw() 
        => DrawConfigurationTab();

    void DrawConfigurationTab();
}