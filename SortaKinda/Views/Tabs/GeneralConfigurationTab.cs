using KamiLib.Interfaces;

namespace SortaKinda.Views.Tabs;

public class GeneralConfigurationTab : ITabItem
{
    public string TabName => "General Settings";
    public bool Enabled => true;
    public void Draw() => SortaKindaPlugin.Controller.DrawConfig();
}