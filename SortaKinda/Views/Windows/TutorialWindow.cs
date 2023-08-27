using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib;
using KamiLib.Interfaces;
using KamiLib.Windows;
using SortaKinda.Views.Tabs;

namespace SortaKinda.Views.Windows;

public class TutorialWindow : Window
{
    private static TutorialWindow? _instance;
    public static TutorialWindow Instance => _instance ??= new TutorialWindow();

    private readonly TabBar tabBar;

    private TutorialWindow() : base("SortaKinda - Tutorial")
    {
        Size = new Vector2(640, 425);

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        Flags |= ImGuiWindowFlags.NoResize;

        tabBar = new TabBar
        {
            Id = "TutorialTabBar",
            TabItems = new ITabItem[]
            {
                new TutorialAboutTab(),
                new TutorialSortingRules(),
                new TutorialConfiguringInventory(),
                new TutorialAdvancedSorting(),
            }
        };
    }

    public void Open()
    {
        if (KamiCommon.WindowManager.GetWindowOfType<TutorialWindow>() is null)
        {
            KamiCommon.WindowManager.AddWindow(this);
            IsOpen = true;
        }
    }

    public override void Draw()
    {
        tabBar.Draw();
    }

    public override void OnClose()
    {
        if (KamiCommon.WindowManager.GetWindowOfType<TutorialWindow>() is not null)
        {
            KamiCommon.WindowManager.RemoveWindow(this);
        }
    }
}