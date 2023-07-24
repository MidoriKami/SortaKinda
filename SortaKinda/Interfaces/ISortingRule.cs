using System.Numerics;

namespace SortaKinda.Interfaces;

public interface ISortingRule
{
    Vector4 Color { get; set; }
    string Id { get; set; }
    string Name { get; set; }
    int Priority { get; set; }
    void DrawTooltip();
}