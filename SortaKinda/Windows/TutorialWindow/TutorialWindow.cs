using System.Numerics;
using ImGuiNET;
using KamiLib.TabBar;
using KamiLib.Window;
using SortaKinda.Controllers;

namespace SortaKinda.Windows.TutorialWindow;

public class TutorialWindow : Window {
    private readonly TabBar tabBar;

    public TutorialWindow() : base("SortaKinda - Tutorial", new Vector2(640.0f, 425.0f)) {

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        tabBar = new TabBar("TutorialTabBar", [
            new TutorialAboutTab(),
            new TutorialSortingRules(),
            new TutorialConfiguringInventory(),
            new TutorialAdvancedSorting()
        ]);
    }

    protected override void DrawContents() 
        => tabBar.Draw();

    public override void OnClose() 
        => SortaKindaController.WindowManager.RemoveWindow(this);
}