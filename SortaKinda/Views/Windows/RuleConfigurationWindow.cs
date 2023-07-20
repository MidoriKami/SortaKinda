using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.Views.Windows;

public class RuleConfigurationWindow : Window
{
    private readonly SortingRule rule;

    public ConfigurationResult Result { get; private set; } = ConfigurationResult.None;
    
    public RuleConfigurationWindow(SortingRule rule) : base($"SortaKinda Rule Configuration##{rule.Id}")
    {
        this.rule = rule;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(350.0f, 500.0f),
            MaximumSize = new Vector2(9999, 9999)
        };
    }

    public override void OnOpen()
    {
        BringToFront();
    }

    public override void Draw()
    {
        rule.DrawConfig();

        ImGuiHelpers.ScaledDummy(10.0f);
            
        var region = ImGui.GetContentRegionMax();
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X - 100.0f - 100.0f - ImGui.GetStyle().ItemSpacing.X });
        if (ImGui.Button("Delete Rule", new Vector2(100.0f, 23.0f)))
        {
            Result = ConfigurationResult.RemoveEntry;
        }

        ImGui.SameLine();
        if (ImGui.Button("Save and Close", new Vector2(100.0f, 23.0f)))
        {
            Result = ConfigurationResult.SaveAndClose;
        }
    }

    public override void OnClose()
    {
        Result = ConfigurationResult.SaveAndClose;
    }
}