using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Abstracts;

namespace SortaKinda.Models;

public class SortingFilter
{
    public HashSet<uint> AllowedItemTypes { get; set; } = new();
    public bool UseSpecificName;
    public string SpecificName = string.Empty;

    private string searchString = string.Empty;
    private List<ItemUICategory>? searchResults;

    public bool IsItemSlotAllowed(InventorySlot slot)
    {
        if (UseSpecificName)
        {
            return string.Equals(slot.LuminaData?.Name.RawString, SpecificName, StringComparison.InvariantCultureIgnoreCase);
        }
        else
        {
            return AllowedItemTypes.Contains(slot.LuminaData?.ItemUICategory.Row ?? uint.MaxValue);
        }
    }
    
    public void Draw()
    {
        ImGui.Checkbox("Only Allow Specific Item by Name", ref UseSpecificName);

        if (!UseSpecificName)
        {
            DrawItemTypeSearch();

            DrawCurrentlyAllowedTypes();
        }
        else
        {
            ImGui.InputText("Item Name", ref SpecificName, 1024);
        }
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
        ImGui.SameLine();
        if (ImGuiComponents.IconButton("##ShowAll", FontAwesomeIcon.Search))
        {
            ImGui.OpenPopup("ShowAllItemUiCategory");
        }

        ImGui.SetNextWindowSizeConstraints(new Vector2(1024, 700), new Vector2(1024, 800));
        if (ImGui.BeginPopup("ShowAllItemUiCategory"))
        {
            ImGui.Columns(4);

            foreach (var result in LuminaCache<ItemUICategory>.Instance.OrderBy(item => item.OrderMajor).ThenBy(item => item.OrderMinor))
            {
                if (result is { RowId: 0, Name.RawString: "" }) continue;

                var enabled = AllowedItemTypes.Contains(result.RowId);
                if (ImGui.Checkbox($"##ItemUiCategory{result.RowId}", ref enabled))
                {
                    if (enabled) AllowedItemTypes.Add(result.RowId);
                    if (!enabled) AllowedItemTypes.Remove(result.RowId);
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