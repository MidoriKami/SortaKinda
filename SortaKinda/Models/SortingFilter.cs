using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace SortaKinda.Models;

public class SortingFilter
{
    public HashSet<uint> AllowedItemTypes { get; set; } = new();

    private string searchString = string.Empty;
    private List<ItemUICategory>? searchResults;

    public void Draw()
    {
        DrawItemTypeSearch();
        
        DrawCurrentlyAllowedTypes();
    }
    
    private void DrawItemTypeSearch()
    {
        ImGui.TextUnformatted("Item Type Search");
        ImGui.Separator();

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
                    .Take(7)
                    .ToList();
            }
        }

        if (searchResults is not null)
        {
            if (ImGui.BeginChild("##SearchResultsChild", new Vector2(0, 180.0f * ImGuiHelpers.GlobalScale)))
            {
                if (searchResults is null || searchResults?.Count is 0)
                {
                    ImGui.TextUnformatted("No Results");
                }
                else if (searchResults is not null)
                {
                    foreach (var result in searchResults)
                    {
                        if (result.Name.RawString is "") continue;

                        if (ImGuiComponents.IconButton($"##AddCategoryButton{result.RowId}", FontAwesomeIcon.Plus))
                        {
                            AllowedItemTypes.Add(result.RowId);
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

    private void DrawCurrentlyAllowedTypes()
    {
        ImGui.TextUnformatted("Allowed Item Types");
        ImGui.Separator();
        uint? removalEntry = null;

        if (AllowedItemTypes.Count is 0)
        {
            ImGui.TextUnformatted("Nothing Allowed");
        }

        foreach (var entry in AllowedItemTypes)
        {
            if (ImGuiComponents.IconButton($"##RemoveButton{entry}", FontAwesomeIcon.Trash))
            {
                removalEntry = entry;
            }

            if (LuminaCache<ItemUICategory>.Instance.GetRow(entry) is not { Icon: var iconId, Name.RawString: var entryName }) continue;

            if (IconCache.Instance.GetIcon((uint) iconId) is { } icon)
            {
                ImGui.SameLine();
                ImGui.Image(icon.ImGuiHandle, new Vector2(20.0f, 20.0f));
            }

            ImGui.SameLine();
            ImGui.TextUnformatted(entryName);
        }

        if (removalEntry is { } toRemove) AllowedItemTypes.Remove(toRemove);
    }
}