using System.Collections.Generic;
using System.Drawing;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using KamiLib.Utilities;

namespace SortaKinda.Views.SortingRuleViews;

public class ItemNameFilterView
{
    private static string _newName = string.Empty;
    private static bool _setNameFocus;
    private static HashSet<string> _allowedNames = null!;

    public static void Draw(HashSet<string> allowedNames)
    {
        _allowedNames = allowedNames;
        
        DrawAllowedItemNames();
        DrawAddNewAllowedItemName();
    }
    
    private static void DrawAllowedItemNames()
    {
        if (ImGui.BeginTable("##NameFilterTable", 1, ImGuiTableFlags.BordersInnerV))
        {
            ImGui.TableNextColumn();
            ImGui.Text("Allowed Item Names");
            ImGui.Separator();

            ImGui.TableNextColumn();
            string? removalString = null;
            if (ImGui.BeginChild("##NameFilterChild", new Vector2(0.0f, -50.0f)))
            {
                if (_allowedNames.Count is 0) ImGui.Text("Nothing Filtered");
                foreach (var name in _allowedNames)
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
            if (removalString is { } toRemove) _allowedNames.Remove(toRemove);

            ImGui.EndTable();
        }
    }
    
    private static void DrawAddNewAllowedItemName()
    {
        if (_setNameFocus)
        {
            ImGui.SetKeyboardFocusHere();
            _setNameFocus = false;
        }

        ImGui.TextColored(KnownColor.Gray.AsVector4(), "Supports Regex for item name filtering");
        
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 25.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.X - 1.0f);
        if (ImGui.InputTextWithHint("##NewName", "Item Name", ref _newName, 1024, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (_newName is not "") _allowedNames.Add(_newName);
            _setNameFocus = true;
        }
        
        ImGui.SameLine();
        
        if (ImGuiComponents.IconButton("##AddNameButton", FontAwesomeIcon.Plus))
        {
            if (_newName is not "") _allowedNames.Add(_newName);
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Add Name");
        }
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}