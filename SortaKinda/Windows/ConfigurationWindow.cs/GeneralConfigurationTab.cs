using KamiLib.TabBar;
using SortaKinda.Controllers;

namespace SortaKinda.Windows.ConfigurationWindow.cs;

public class GeneralConfigurationTab : ITabItem {
    public string Name => "General Settings";
    
    public bool Disabled => false;
    
    public void Draw() 
        => SortaKindaController.DrawConfig();
}