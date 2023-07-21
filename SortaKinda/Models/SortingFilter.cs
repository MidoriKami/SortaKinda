using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
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
    public HashSet<string> AllowedNames { get; set; } = new();

    private string newName = string.Empty;
    private string searchString = string.Empty;
    private List<ItemUICategory>? searchResults;
    private bool setNameFocus;

    public bool IsItemSlotAllowed(InventorySlot slot)
    {
        if (AllowedNames.Count > 0 && !AllowedNames.Any(allowed => Regex.IsMatch(slot.LuminaData?.Name.RawString ?? string.Empty, allowed, RegexOptions.IgnoreCase))) return false;
        if (AllowedItemTypes.Count > 0 && !AllowedItemTypes.Any(allowed => slot.LuminaData?.ItemUICategory.Row == allowed)) return false;

        return true;
    }

    public void DrawConfig()
    {
        if (ImGui.BeginTabItem("Item Name Filter"))
        {
            DrawAddRemoveNameFilter();
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Item Type Filter"))
        {
            DrawAddRemoveItemTypeFilter();
            ImGui.EndTabItem();
        }
    }
    
    private void DrawAddRemoveNameFilter()
    {
        if (ImGui.BeginTable("##NameFilterTable", 1, ImGuiTableFlags.BordersInnerV))
        {
            ImGui.TableNextColumn();
            ImGui.Text("Allowed Item Names");
            ImGui.Separator();
            
            ImGui.TableNextColumn();
            string? removalString = null;
            if (ImGui.BeginChild("##NameFilterChild", new Vector2(0.0f, -30.0f)))
            {
                if (AllowedNames.Count is 0) ImGui.Text("Nothing Filtered");
                foreach (var name in AllowedNames)
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
            if (removalString is { } toRemove) AllowedNames.Remove(toRemove);

            ImGui.EndTable();
        }

        if (setNameFocus)
        {
            ImGui.SetKeyboardFocusHere();
            setNameFocus = false;
        }

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 25.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.X);
        if (ImGui.InputTextWithHint("##NewName", "Item Name", ref newName, 1024, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (newName is not "") AllowedNames.Add(newName);
            setNameFocus = true;
        }
        ImGui.SameLine();
        if (ImGuiComponents.IconButton("##AddNameButton", FontAwesomeIcon.Plus))
        {
            AllowedNames.Add(newName);
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Add Name");
        }
        ImGuiHelpers.ScaledIndent(-15.0f);
    }

    private void DrawAddRemoveItemTypeFilter()
    {
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
            if (ImGui.BeginChild("##ItemFilterChild", new Vector2(0.0f, 0.0f), false))
            {
                if(AllowedItemTypes.Count is 0) ImGui.Text("Nothing Filtered");
                foreach (var category in AllowedItemTypes)
                {
                    if (LuminaCache<ItemUICategory>.Instance.GetRow(category) is not { Icon: var iconCategory, Name.RawString: var entryName }) continue;
                    if (IconCache.Instance.GetIcon((uint)iconCategory) is not {} iconTexture ) continue;
                
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
            if (removalEntry is { } toRemove) AllowedItemTypes.Remove(toRemove);

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 25.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.X);
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

            if (ImGui.BeginChild("##SearchResultsChild", new Vector2(0, 0.0f), false))
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

                        if (!AllowedItemTypes.Contains(result.RowId))
                        {
                            if (ImGuiComponents.IconButton($"##AddCategoryButton{result.RowId}", FontAwesomeIcon.Plus))
                            {
                                AllowedItemTypes.Add(result.RowId);
                            }
                        }
                        else
                        {
                            if (ImGuiComponents.IconButton($"##RemoveCategoryButton{result.RowId}", FontAwesomeIcon.Trash))
                            {
                                AllowedItemTypes.Remove(result.RowId);
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

            ImGui.EndTable();
        }
    }
}