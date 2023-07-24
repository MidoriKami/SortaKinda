using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace SortaKinda.Views.SortingRuleViews;

public class ItemTypeFilterView
{
    private static HashSet<uint> _allowedItemTypes = null!;
    private static string _searchString = string.Empty;
    private static List<ItemUICategory>? _searchResults;

    public static void Draw(HashSet<uint> allowedItemTypes)
    {
        _allowedItemTypes = allowedItemTypes;
        uint? removalEntry = null;

        if (ImGui.BeginTable("##ItemTypeTable", 2, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.BordersInnerV))
        {
            ImGui.TableNextColumn();
            ImGui.Text("Allowed Item Types");
            ImGui.Separator();

            ImGui.TableNextColumn();
            ImGui.Text("Item Type Search");
            ImGui.Separator();

            ImGui.TableNextColumn();
            DrawFilteredItemTypes(removalEntry);

            ImGui.TableNextColumn();
            DrawItemTypeSearchBox();
            DrawAllItemTypesPopup();
            DrawItemTypeSearchResults();

            ImGui.EndTable();
        }
    }
    
    private static void DrawFilteredItemTypes(uint? removalEntry)
    {
        if (ImGui.BeginChild("##ItemFilterChild", new Vector2(0.0f, 0.0f), false))
        {
            if (_allowedItemTypes.Count is 0) ImGui.Text("Nothing Filtered");
            foreach (var category in _allowedItemTypes)
            {
                if (LuminaCache<ItemUICategory>.Instance.GetRow(category) is not { Icon: var iconCategory, Name.RawString: var entryName }) continue;
                if (IconCache.Instance.GetIcon((uint) iconCategory) is not { } iconTexture) continue;

                if (ImGuiComponents.IconButton($"##RemoveButton{category}", FontAwesomeIcon.Trash))
                {
                    removalEntry = category;
                }

                ImGui.SameLine();
                ImGui.SetCursorPos(ImGui.GetCursorPos() with { Y = ImGui.GetCursorPos().Y + 2.0f });
                ImGui.Image(iconTexture.ImGuiHandle, new Vector2(20.0f, 20.0f));

                ImGui.SameLine();
                ImGui.TextUnformatted(entryName);
            }
        }
        ImGui.EndChild();
        if (removalEntry is { } toRemove) _allowedItemTypes.Remove(toRemove);
    }
    
    private static void DrawItemTypeSearchBox()
    {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 25.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.X);
        if (ImGui.InputTextWithHint("##SearchBox", "Search...", ref _searchString, 1024, ImGuiInputTextFlags.AutoSelectAll))
        {
            if (_searchString.IsNullOrEmpty())
            {
                _searchResults = null;
            }
            else
            {
                _searchResults = LuminaCache<ItemUICategory>.Instance
                    .Where(entry => entry.Name.RawString.ToLowerInvariant().Contains(_searchString.ToLowerInvariant()))
                    .ToList();
            }
        }
        ImGui.SameLine();
        if (ImGuiComponents.IconButton("##ShowAll", FontAwesomeIcon.Search))
        {
            ImGui.OpenPopup("ShowAllItemUiCategory");
        }
    }
    
    private static void DrawAllItemTypesPopup()
    {
        ImGui.SetNextWindowSizeConstraints(new Vector2(1024, 700), new Vector2(1024, 800));
        if (ImGui.BeginPopup("ShowAllItemUiCategory"))
        {
            ImGui.Columns(4);

            foreach (var result in LuminaCache<ItemUICategory>.Instance.OrderBy(item => item.OrderMajor).ThenBy(item => item.OrderMinor))
            {
                if (result is { RowId: 0, Name.RawString: "" }) continue;

                var enabled = _allowedItemTypes.Contains(result.RowId);
                if (ImGui.Checkbox($"##ItemUiCategory{result.RowId}", ref enabled))
                {
                    if (enabled) _allowedItemTypes.Add(result.RowId);
                    if (!enabled) _allowedItemTypes.Remove(result.RowId);
                }

                if (IconCache.Instance.GetIcon((uint) result.Icon) is { } icon)
                {
                    ImGui.SameLine();
                    ImGui.SetCursorPos(ImGui.GetCursorPos() with { Y = ImGui.GetCursorPos().Y + 2.0f });
                    ImGui.Image(icon.ImGuiHandle, new Vector2(20.0f, 20.0f));
                }

                ImGui.SameLine();
                ImGui.TextUnformatted(result.Name.RawString);

                ImGui.NextColumn();
            }

            ImGui.Columns(1);
            ImGui.EndPopup();
        }
    }
    
    private static void DrawItemTypeSearchResults()
    {
        if (ImGui.BeginChild("##SearchResultsChild", new Vector2(0, 0.0f), false))
        {
            if (_searchResults is null || _searchResults.Count is 0)
            {
                ImGui.TextUnformatted("No Results");
            }
            else
            {
                foreach (var result in _searchResults.Where(result => result.Name.RawString is not ""))
                {
                    if (!_allowedItemTypes.Contains(result.RowId))
                    {
                        if (ImGuiComponents.IconButton($"##AddCategoryButton{result.RowId}", FontAwesomeIcon.Plus))
                        {
                            _allowedItemTypes.Add(result.RowId);
                        }
                    }
                    else
                    {
                        if (ImGuiComponents.IconButton($"##RemoveCategoryButton{result.RowId}", FontAwesomeIcon.Trash))
                        {
                            _allowedItemTypes.Remove(result.RowId);
                        }
                    }

                    if (IconCache.Instance.GetIcon((uint) result.Icon) is { } icon)
                    {
                        ImGui.SameLine();
                        ImGui.SetCursorPos(ImGui.GetCursorPos() with { Y = ImGui.GetCursorPos().Y + 2.0f });
                        ImGui.Image(icon.ImGuiHandle, new Vector2(20.0f, 20.0f));
                    }

                    ImGui.SameLine();
                    ImGui.TextUnformatted(result.Name.RawString);
                }
            }
        }
        ImGui.EndChild();
    }
}