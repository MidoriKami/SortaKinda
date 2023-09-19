using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Interfaces;

namespace SortaKinda.Views.Tabs;

public class ItemNameFilterTab : IOneColumnRuleConfigurationTab
{
    private string newName = string.Empty;
    private bool setNameFocus = true;

    public ItemNameFilterTab(ISortingRule rule)
    {
        SortingRule = rule;
    }
    public string TabName => "Item Name Filter";
    public bool Enabled => true;
    public string FirstLabel => "Allowed Item Names";
    public ISortingRule SortingRule { get; }

    public void DrawContents()
    {
        DrawFilteredNames();
        DrawAddItemNameInput();
    }

    private void DrawFilteredNames()
    {
        string? removalString = null;
        if (ImGui.BeginChild("##NameFilterChild", new Vector2(0.0f, -50.0f)))
        {
            if (SortingRule.AllowedItemNames.Count is 0)
            {
                ImGui.TextColored(KnownColor.Orange.AsVector4(), "Nothing Filtered");
            }

            foreach (var name in SortingRule.AllowedItemNames)
            {
                if (ImGuiComponents.IconButton($"##RemoveName{name}", FontAwesomeIcon.Trash))
                {
                    removalString = name;
                }
                ImGui.SameLine();
                ImGui.TextUnformatted(name);
            }
        }
        ImGui.EndChild();

        if (removalString is { } toRemove)
        {
            SortingRule.AllowedItemNames.Remove(toRemove);
        }
    }

    private void DrawAddItemNameInput()
    {
        var buttonSize = ImGuiHelpers.ScaledVector2(25.0f, 23.0f);

        if (setNameFocus || ImGui.IsWindowAppearing())
        {
            ImGui.SetKeyboardFocusHere();
            setNameFocus = false;
        }

        ImGui.TextColored(KnownColor.Gray.AsVector4(), "Supports Regex for item name filtering");

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttonSize.X - ImGui.GetStyle().ItemSpacing.X);
        if (ImGui.InputTextWithHint("##NewName", "Item Name", ref newName, 1024, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (newName is not "")
            {
                SortingRule.AllowedItemNames.Add(newName);
            }
            setNameFocus = true;
        }

        ImGui.SameLine();

        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##AddNameButton", buttonSize))
        {
            if (newName is not "")
            {
                SortingRule.AllowedItemNames.Add(newName);
            }
        }
        ImGui.PopFont();

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Add Name");
        }
    }
}