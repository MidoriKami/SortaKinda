using System.Numerics;
using SortaKinda.Interfaces;
using SortaKinda.System;

namespace SortaKinda.Models;

public class SortingRule : ISortingRule
{
    public Vector4 Color { get; set; }
    public string Id { get; set; } = SortController.DefaultId;
    public string Name { get; set; } = "New Rule";
    public int Priority { get; set; }
    
    public void DrawTooltip()
    {
    }
}