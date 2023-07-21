using System;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Interfaces;

namespace SortaKinda.Models;

public class SortingRule : ISortingRule
{
    public Vector4 Color { get; set; } = KnownColor.White.AsVector4();
    public string Id { get; init; } = "Default";
    public string Name { get; set; } = "Unsorted";
    public int Priority { get; set; }
    public ISortingFilter Filter { get; set; } = new SortingFilter();
    public ISortingOrder Order { get; set; } = new SortingOrder();

    public bool Equals(SortingRule? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public void DrawListEntry()
    {
        var imGuiColor = Color;
        ImGui.ColorEdit4("##ColorTooltip", ref imGuiColor, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoPicker);
        ImGui.SameLine();
        ImGui.Text(Name);
    }

    public void DrawTooltip()
    {
        ImGui.BeginTooltip();
        var imGuiColor = Color;
        if (ImGui.ColorEdit4("##ColorTooltip", ref imGuiColor, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoPicker)) Color = imGuiColor;
        ImGui.SameLine();
        ImGui.Text(Name);

        if (Name is not "Unsorted")
        {
            var allowedItems = GetAllowedItemsString();

            ImGui.TextColored(KnownColor.Gray.AsVector4(), allowedItems[..Math.Min(allowedItems.Length, 55)]);
            ImGui.TextColored(KnownColor.Gray.AsVector4(), GetSortingModesString());
        }

        ImGui.EndTooltip();
    }

    public void DrawConfig()
    {
        Filter.DrawConfigTabs();
        Order.DrawConfigTabs();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((SortingRule) obj);
    }

    private string GetAllowedItemsString()
    {
        return Filter.GetAllowedItemsString();
    }

    private string GetSortingModesString()
    {
        return Order.GetSortingModeString();
    }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}