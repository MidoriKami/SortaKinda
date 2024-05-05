using KamiLib.TabBar;
using SortaKinda.System;

namespace SortaKinda.Views.Tabs;

public class GeneralConfigurationTab : ITabItem {
    public string Name => "General Settings";
    
    public bool Disabled => false;
    
    public void Draw() 
        => SortaKindaController.DrawConfig();
}