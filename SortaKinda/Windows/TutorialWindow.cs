using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using KamiLib.Classes;
using KamiLib.Window;

namespace SortaKinda.Windows;

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
        => System.WindowManager.RemoveWindow(this);
}

public class TutorialAboutTab : ITabItem {
    public string Name => "About";
    
    public bool Disabled => false;
    
    public void Draw() {
        ImGuiHelpers.ScaledDummy(10.0f);
        
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(ImGui.GetStyle().ItemSpacing.X, 10.0f * ImGuiHelpers.GlobalScale));
        
        ImGui.TextWrapped(AboutText);
        
        ImGui.PopStyleVar();
    }

    private const string AboutText = "Welcome to SortaKinda! A highly customizable inventory management tool.\n" +
                                     "This plugin was designed for you to define precisely what items you want to always be in specific sections of your inventory.\n\n" +
                                     "SortaKinda has no relation to the built in 'isort' function. It does not interact with 'isort' in any way.\n" +
                                     "SortaKinda will override any other sorting systems you attempt to use.\n\n" +
                                     "Automatic sort triggers are available in the general settings tab, these triggers allow SortaKinda to automatically re-sort your inventory as you are playing.\n\n" +
                                     "There may be times where SortaKinda might not catch a change, worry not as triggers should be frequent enough to sort things out on the next change.";
}

public class TutorialSortingRules : ITabItem {
    public string Name => "Sorting Rules";
    
    public bool Disabled => false;
    
    public void Draw() {
        ImGuiHelpers.ScaledDummy(10.0f);
        
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(ImGui.GetStyle().ItemSpacing.X, 10.0f * ImGuiHelpers.GlobalScale));
        
        ImGui.TextWrapped(SortingRulesHelp);
        
        ImGui.PopStyleVar();
    }

    private const string SortingRulesHelp = "Sorting rules are definitions for what items you want to allow into specific inventory slots.\n" +
                                            "Rules are shown on the left side of the configuration window, new rules are added to the bottom of the list.\n\n" +
                                            "Rules can be deleted by pressing and holding Shift and Control at the same time, then clicking the trash icon. The 'Unsorted' rule can not be deleted.\n\n" +
                                            "If multiple types of filters are used in the same rule, then an item will only be allowed if all filter types permit it.\n\n" +
                                            "For example, if you set a rule with ItemType: Miscallany, and ItemRarity: Green, then only items that are both Miscellany AND Green rarity will be allowed in those slots.\n\n" +
                                            "Items that don't match any rules will be moved into the 'Unsorted' sections of your inventory. You must always have some inventory slots marked as 'Unsorted'.\n\n" +
                                            "If SortaKinda is unable to move an item out of a sorted section into a 'Unsorted' section, then it will act as if that item belongs in the sorted section and order it based on that sections rule.";
}

public class TutorialConfiguringInventory : ITabItem {
    public string Name => "Using Rules";
    
    public bool Disabled => false;
    
    public void Draw() {
        ImGuiHelpers.ScaledDummy(10.0f);
        
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(ImGui.GetStyle().ItemSpacing.X, 10.0f * ImGuiHelpers.GlobalScale));
        
        ImGui.TextWrapped(UsingRulesHelp);
        
        ImGui.PopStyleVar();
    }

    private const string UsingRulesHelp = "To use rules, you need to first select a rule on the left side of the configuration window.\n" +
                                          "The currently selected rule will have a filledin dot next to the color/name of the rule.\n\n" +
                                          "Once you have a rule selected you can 'paint' your inventory slots on the right side of the configuration window. Painted slots do not have to be adjacent to each other.\n\n" +
                                          "Whenever a sort is triggered, items that match the rules of a painted inventory slot will try to be moved into those inventory slots, and then re-ordered according to the rule's settings.\n" +
                                          "Items that don't match a rule but are in a rules inventory slots will be moved out into 'Unsorted' slots.\n\n" +
                                          "You must always have some inventory slots marked as 'Unsorted'.";
}

public class TutorialAdvancedSorting : ITabItem {
    public string Name => "Advanced Techniques";
    
    public bool Disabled => false;
    
    public void Draw() {
        ImGuiHelpers.ScaledDummy(10.0f);
        
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(ImGui.GetStyle().ItemSpacing.X, 10.0f * ImGuiHelpers.GlobalScale));
        
        ImGui.TextWrapped(AdvancedTech);

        ImGui.PopStyleVar();
    }

    private const string AdvancedTech = "SortaKinda evaluates sorting rules in a specific order, " +
                                        "this allows you to define rules in such a way that you can have items that match multiple rules always end up in one specific section of your inventory.\n\n" +
                                        "Rules are evaluated from the top of the list (where 'Unsorted' is), to the bottom of the list (where the add new rule button is), " +
                                        "If an item would be allowed by multiple rules, the rule lowest in the list will get the item in the end.\n\n" +
                                        "You can use this characteristic of SortaKinda to define generalized sorting rules at the top of the list, and more specific sorting rules at the bottom of the list, " +
                                        "any items that match the more specific rules at the bottom will have the items in the end.\n\n" +
                                        "In other words, you can consider the order the rules are in to be a soft-priority system, where the rules on the bottom are more important.";
}