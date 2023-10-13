using System.Numerics;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Interfaces;

namespace SortaKinda.Views.Tabs;

public class TutorialAdvancedSorting : ITabItem
{
    public string TabName => "Advanced Techniques";
    public bool Enabled => true;
    public void Draw()
    {
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