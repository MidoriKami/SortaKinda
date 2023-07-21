using System;
using System.Numerics;
using SortaKinda.Interfaces;

namespace SortaKinda.Models;

public interface ISortingRule : IEquatable<SortingRule>
{
    Vector4 Color { get; set; }
    string Id { get; init; }
    string Name { get; set; }

    ISortingFilter Filter { get; set; }
    ISortingOrder Order { get; set; }

    bool Equals(object? obj);
    void DrawListEntry();
    void DrawTooltip();
    void DrawConfig();
}