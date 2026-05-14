using System.Drawing;
using System.Numerics;
using Dalamud.Interface;

namespace SortaKinda.Classes;

/// <summary>
/// Intended for Dev Use to easily tweak how inventory slots are drawn by ImGui.
/// </summary>
public record struct DrawOptions() {
	public Vector4 OutlineColor { get; set; } = KnownColor.White.Vector() with { W = 0.15f };
	public float BorderThickness { get; set; } = 2.0f;
}