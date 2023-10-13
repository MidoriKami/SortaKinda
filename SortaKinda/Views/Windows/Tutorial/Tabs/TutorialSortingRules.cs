using System.Numerics;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Interfaces;

namespace SortaKinda.Views.Tabs;

public class TutorialSortingRules : ITabItem
{
    public string TabName => "Sorting Rules";
    public bool Enabled => true;
    public void Draw()
    {
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