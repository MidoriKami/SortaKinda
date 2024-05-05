using System.Numerics;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.TabBar;

namespace SortaKinda.Views.Tabs;

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