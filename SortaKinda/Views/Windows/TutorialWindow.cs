using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib;

namespace SortaKinda.Views.Windows;

public class TutorialWindow : Window
{
    private static TutorialWindow? _instance;
    public static TutorialWindow Instance => _instance ??= new TutorialWindow();
    
    public TutorialWindow() : base("SortaKinda - Tutorial")
    {
        Size = new Vector2(600, 636);

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        Flags |= ImGuiWindowFlags.NoResize;
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
        ImGui.Text("About SortaKinda");
        ImGui.Separator();
        DrawAbout();
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.Text("Sorting Rules");
        ImGui.Separator();
        DrawRuleHelp();
    }
    
    private void DrawAbout()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(ImGui.GetStyle().ItemSpacing.X, 10.0f * ImGuiHelpers.GlobalScale));
        
        ImGui.TextWrapped("Welcome to SortaKinda! A highly customizable inventory management tool.");
        ImGui.TextWrapped("This plugin was designed for you to define precisely what items you want to always be in specific sections of your inventory.");
        ImGui.TextWrapped("SortaKinda has no relation to the ingame's 'isort' function. It does not interact with that system at all. SortaKinda will override any other sorting system you attempt to use.");
        ImGui.TextWrapped("Automatic sort triggers are available in the General Settings tab that allow SortaKinda to automatically re-sort your inventory as you are playing.");
        ImGui.TextWrapped("There may be times where SortaKinda might not catch a change, worry not as triggers should be frequent enough to sort things out on the next change.");
        
        ImGui.PopStyleVar();
    }
    
    private void DrawRuleHelp()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(ImGui.GetStyle().ItemSpacing.X, 10.0f * ImGuiHelpers.GlobalScale));
        
        ImGui.TextWrapped("The left side of the configuration window shows the available sorting rules. This is a library of rules that you can use.");
        ImGui.TextWrapped("You can export and share rules, or import rules from an external source. Once the import is complete a chat message will be displayed with the result of the import.");
        ImGui.TextWrapped("Rules are used as filters to define what items are supposed to be allowed, once a rule is defined you paint the rule onto the inventory grid. This will tell SortaKinda that those inventory slots should contain the defined items.");
        ImGui.TextWrapped("Anytime a sort is triggered, SortaKinda will scan your entire inventory, and try to move any items that match any rules into the inventory slots set for that rule.");
        ImGui.TextWrapped("Any items that are in a painted section of the inventory that don't match that rule will be moved out into 'Unsorted' slots.");
        ImGui.TextWrapped("You must always have some slots marked as 'Unsorted'. If SortaKinda is unable to move an item out of a painted inventory slot it will act as if that item belongs there, and order it based on that slots rules.");
        
        ImGui.PopStyleVar();
    }

    public override void OnClose()
    {
        if (KamiCommon.WindowManager.GetWindowOfType<TutorialWindow>() is not null)
        {
            KamiCommon.WindowManager.RemoveWindow(this);
        }
    }
}