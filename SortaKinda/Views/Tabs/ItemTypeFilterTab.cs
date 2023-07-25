using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib;
using KamiLib.Caching;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.Views.Windows;

namespace SortaKinda.Views.Tabs;

public class ItemTypeFilterTab : ITwoColumnRuleConfigurationTab
{
    public string TabName => "Item Type Filter";
    public string FirstLabel => "Allowed Item Types";
    public string SecondLabel => "Item Type Search";
    public bool Enabled => true;
    
    public ISortingRule SortingRule { get; set; }
    private string searchString = string.Empty;
    private List<ItemUICategory>? searchResults;

    public ItemTypeFilterTab(ISortingRule rule)
    {
        SortingRule = rule;
    }
    
    public void DrawLeftSideContents()
    {
        uint? removalEntry = null;
        if (ImGui.BeginChild("##ItemFilterChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding, false))
        {
            if (SortingRule.AllowedItemTypes.Count is 0)
            {
                ImGui.TextColored(KnownColor.Orange.AsVector4(), "Nothing Filtered");
            }

            foreach (var category in SortingRule.AllowedItemTypes)
            {
                if (LuminaCache<ItemUICategory>.Instance.GetRow(category) is not { Icon: var iconCategory, Name.RawString: var entryName }) continue;
                if (IconCache.Instance.GetIcon((uint) iconCategory) is not { } iconTexture) continue;

                if (ImGuiComponents.IconButton($"##RemoveButton{category}", FontAwesomeIcon.Trash))
                {
                    removalEntry = category;
                }

                ImGui.SameLine();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1.0f * ImGuiHelpers.GlobalScale);
                ImGui.Image(iconTexture.ImGuiHandle, ImGuiHelpers.ScaledVector2(20.0f, 20.0f));

                ImGui.SameLine();
                ImGui.TextUnformatted(entryName);
            }
        }
        ImGui.EndChild();
        
        if (removalEntry is { } toRemove)
        {
            SortingRule.AllowedItemTypes.Remove(toRemove);
        }
    }
    
    public void DrawRightSideContents()
    {
        var buttonSize = ImGuiHelpers.ScaledVector2(23.0f, 23.0f);
        
        if (ImGui.IsWindowAppearing())
        {
            ImGui.SetKeyboardFocusHere();
        }
        
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttonSize.X - ImGui.GetStyle().ItemSpacing.X);
        if (ImGui.InputTextWithHint("##SearchBox", "Search...", ref searchString, 1024, ImGuiInputTextFlags.AutoSelectAll))
        {
            if (searchString.IsNullOrEmpty())
            {
                searchResults = null;
            }
            else
            {
                searchResults = LuminaCache<ItemUICategory>.Instance
                    .Where(entry => entry.Name.RawString.ToLowerInvariant().Contains(searchString.ToLowerInvariant()))
                    .ToList();
            }
        }
        
        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        if(ImGui.Button($"{FontAwesomeIcon.Search.ToIconString()}##ShowAll", buttonSize))
        {
            KamiCommon.WindowManager.AddWindow(new ItemTypeConfigWindow(SortingRule));
        }
        ImGui.PopFont();
        
        if (ImGui.BeginChild("##SearchResultsChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding, false))
        {
            if (searchResults is null || searchResults.Count is 0)
            {
                ImGui.TextColored(KnownColor.Gray.AsVector4(), "No Results");
            }
            else
            {
                foreach (var result in searchResults.Where(result => result.Name.RawString is not ""))
                {
                    if (!SortingRule.AllowedItemTypes.Contains(result.RowId))
                    {
                        if (ImGuiComponents.IconButton($"##AddCategoryButton{result.RowId}", FontAwesomeIcon.Plus))
                        {
                            SortingRule.AllowedItemTypes.Add(result.RowId);
                        }
                    }
                    else
                    {
                        if (ImGuiComponents.IconButton($"##RemoveCategoryButton{result.RowId}", FontAwesomeIcon.Trash))
                        {
                            SortingRule.AllowedItemTypes.Remove(result.RowId);
                        }
                    }

                    if (IconCache.Instance.GetIcon((uint) result.Icon) is { } icon)
                    {
                        ImGui.SameLine();
                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 1.0f * ImGuiHelpers.GlobalScale);
                        ImGui.Image(icon.ImGuiHandle, ImGuiHelpers.ScaledVector2(20.0f, 20.0f));
                    }

                    ImGui.SameLine();
                    ImGui.TextUnformatted(result.Name.RawString);
                }
            }
        }
        ImGui.EndChild();
    }
}