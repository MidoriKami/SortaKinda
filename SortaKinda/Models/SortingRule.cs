using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;

namespace SortaKinda.Models;

public class SortingRule : IEquatable<SortingRule>
{
    public required string Id;
    public Vector4 Color = KnownColor.White.AsVector4();
    public string Name = "Unsorted";
    public SortingFilter Filter = new();
    public SortingOrder Order = new();

    public void DrawListEntry()
    {
        ImGui.ColorEdit4("##ColorTooltip", ref Color, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoPicker);
        ImGui.SameLine();
        ImGui.Text(Name);
    }
    
    public void DrawTooltip()
    {
        ImGui.BeginTooltip();
        ImGui.ColorEdit4("##ColorTooltip", ref Color, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoPicker);
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

    private string GetAllowedItemsString()
    {
        var strings = Filter.AllowedItemTypes
            .Select(type => LuminaCache<ItemUICategory>.Instance.GetRow(type)?.Name.RawString ?? "Unknown Type")
            .ToList();

        return strings.Count is 0 ? "Any Item" : string.Join(", ", strings);
    }

    private string GetSortingModesString() => Order.Mode.GetLabel();

    public bool Equals(SortingRule? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((SortingRule) obj);
    }
    
    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Id.GetHashCode();
}