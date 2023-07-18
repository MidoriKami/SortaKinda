using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Models.Enum;

namespace SortaKinda.Models;

public class SortingRule : IEquatable<SortingRule>
{
    public required string Id;
    public Vector4 Color = KnownColor.White.AsVector4();
    public string Name = "Unsorted";
    public SortingFilter Filter = new();
    public SortingOrder Order = new();

    public void DrawHeader()
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
            ImGui.TextColored(KnownColor.Gray.AsVector4(), GetAllowedItemsString());
            ImGui.TextColored(KnownColor.Gray.AsVector4(), GetSortingModesString());
        }

        ImGui.EndTooltip();
    }

    public ConfigurationResult DrawConfigurationWindow()
    {
        var result = ConfigurationResult.None;
        
        if (ImGui.BeginPopup($"{Id}Config"))
        {
            DrawConfig();

            ImGuiHelpers.ScaledDummy(10.0f);
            
            var region = ImGui.GetContentRegionMax();
            ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X - 100.0f - 100.0f - ImGui.GetStyle().ItemSpacing.X });
            if (ImGui.Button("Delete Rule", new Vector2(100.0f, 23.0f)))
            {
                ImGui.CloseCurrentPopup();
                result = ConfigurationResult.RemoveEntry;
            }

            ImGui.SameLine();
            if (ImGui.Button("Save and Close", new Vector2(100.0f, 23.0f)))
            {
                ImGui.CloseCurrentPopup();
                result = ConfigurationResult.SaveAndClose;
            }
                
            ImGui.EndPopup();
        }
        
        return result;
    }

    public void DrawConfig()
    {
        ImGui.TextUnformatted($"Unique ID: {Id}");
        ImGui.ColorEdit4("##ColorConfig", ref Color, ImGuiColorEditFlags.NoInputs);
        
        ImGui.SameLine();
        ImGui.InputText("##NameEdit", ref Name, 1024);

        ImGuiHelpers.ScaledDummy(5.0f);
        Filter.Draw();
        
        ImGuiHelpers.ScaledDummy(5.0f);
        Order.Draw();
    }

    private string GetAllowedItemsString()
    {
        var strings = Filter.AllowedItemTypes
            .Select(type => LuminaCache<ItemUICategory>.Instance.GetRow(type)?.Name.RawString ?? "Unknown Type")
            .ToList();

        return strings.Count is 0 ? "Any Item" : string.Join(", ", strings);
    }

    private string GetSortingModesString() => Order.Mode.ToString();

    public bool Equals(SortingRule? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((SortingRule) obj);
    }
    
    public override int GetHashCode() => Name.GetHashCode();
}