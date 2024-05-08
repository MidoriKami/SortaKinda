using System;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Classes;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Models.Enums;
using SortaKinda.Models.Inventory;

namespace SortaKinda.Models.General;

public class ToggleFilter(PropertyFilter filter, ToggleFilterState state = ToggleFilterState.Ignored) {
    public ToggleFilterState State = state;
    public PropertyFilter Filter = filter;

    public void DrawConfig() {
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3.0f * ImGuiHelpers.GlobalScale);
        ImGui.TextUnformatted(Filter.GetDescription());
        
        ImGui.SameLine(ImGui.GetContentRegionMax().X / 2.0f);
        
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3.0f * ImGuiHelpers.GlobalScale);
        ImGui.PushItemWidth(ImGui.GetContentRegionMax().X / 2.0f);
        using var combo = ImRaii.Combo($"##{Filter.ToString()}Combo", State.GetDescription());
        if (!combo) return;
        
        foreach(var value in Enum.GetValues<ToggleFilterState>()) {
            if (ImGui.Selectable(value.GetDescription(), value == State)) {
                State = value;
            }
        }
    }

    public bool IsItemSlotAllowed(InventorySlot slot) => State switch {
        ToggleFilterState.Ignored => false,
        ToggleFilterState.Allow => ItemHasProperty(slot.ExdItem),
        ToggleFilterState.Disallow => !ItemHasProperty(slot.ExdItem),
        _ => true,
    };

    private bool ItemHasProperty(Item? item) =>  Filter switch {
        PropertyFilter.Collectable when item?.IsCollectable is true => true,
        PropertyFilter.Dyeable when item?.IsDyeable is true => true,
        PropertyFilter.Unique when item?.IsUnique is true => true,
        PropertyFilter.Untradable when item?.IsUntradable is true => true,
        PropertyFilter.Repairable when item?.ItemRepair.Row is not 0 => true,
        _ => false,
    };
}