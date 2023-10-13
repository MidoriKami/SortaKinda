using System;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Utility;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.Models.Enums;

namespace SortaKinda.Models.General;

public class ToggleFilter
{
    public ToggleFilterState State;
    public PropertyFilter Filter;

    public ToggleFilter(PropertyFilter filter)
    {
        Filter = filter;
    }

    public void DrawConfig()
    {
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3.0f * ImGuiHelpers.GlobalScale);
        ImGui.TextUnformatted(Filter.Label());
        
        ImGui.SameLine(ImGui.GetContentRegionMax().X / 2.0f);
        
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3.0f * ImGuiHelpers.GlobalScale);
        ImGui.PushItemWidth(ImGui.GetContentRegionMax().X / 2.0f);
        if (ImGui.BeginCombo($"##{Filter.ToString()}Combo", State.Label()))
        {
            foreach(var value in Enum.GetValues<ToggleFilterState>())
            {
                if (ImGui.Selectable(value.Label(), value == State))
                {
                    State = value;
                }
            }
            
            ImGui.EndCombo();
        }
    }

    public bool IsItemSlotAllowed(IInventorySlot slot) => State switch
    {
        ToggleFilterState.Ignored => false,
        ToggleFilterState.Allow => ItemHasProperty(slot.ExdItem),
        ToggleFilterState.Disallow => !ItemHasProperty(slot.ExdItem),
        _ => true,
    };

    private bool ItemHasProperty(Item? item) =>  Filter switch
    {
        PropertyFilter.Collectable when item?.IsCollectable == true => true,
        PropertyFilter.Dyeable when item?.IsDyeable == true => true,
        PropertyFilter.Unique when item?.IsUnique == true => true,
        PropertyFilter.Untradable when item?.IsUntradable == true => true,
        _ => false,
    };
}