using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Models;
using SortaKinda.Models.Enum;
using SortaKinda.System;

namespace SortaKinda.Views.Windows;

public class RuleConfigurationWindow : Window
{
    private readonly ISortingRule rule;

    public RuleConfigurationWindow(ISortingRule rule) : base($"SortaKinda Rule Configuration - {rule.Name}###{rule.Id}")
    {
        this.rule = rule;

        Position = ImGui.GetMainViewport().Size / 2.0f - new Vector2(500.0f, 400.0f) / 2.0f;
        PositionCondition = ImGuiCond.Appearing;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500.0f, 400.0f),
            MaximumSize = new Vector2(9999, 9999)
        };
    }

    public ConfigurationResult Result { get; private set; } = ConfigurationResult.None;

    public override void Draw()
    {
        DrawHeader();
        DrawBody();
        DrawFooter();
    }

    private void DrawHeader()
    {
        var region = ImGui.GetContentRegionAvail();
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X / 4.0f - ImGuiHelpers.GlobalScale * 50.0f + ImGui.GetStyle().ItemSpacing.X / 2.0f });
        var imGuiColor = rule.Color;
        if (ImGui.ColorEdit4("##ColorConfig", ref imGuiColor, ImGuiColorEditFlags.NoInputs)) rule.Color = imGuiColor;

        ImGui.SameLine();
        ImGui.SetNextItemWidth(region.X / 2.0f - ImGui.GetItemRectSize().X - ImGui.GetStyle().ItemSpacing.X);
        var imGuiName = rule.Name;
        if (ImGui.InputText("##NameEdit", ref imGuiName, 1024, ImGuiInputTextFlags.AutoSelectAll))
        {
            rule.Name = imGuiName;
            WindowName = $"SortaKinda Rule Configuration - {rule.Name}###{rule.Id}";
        }

        var hotkeyHeld = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyAlt;
        if (!hotkeyHeld) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
        ImGui.SameLine();
        if (ImGui.Button("Delete", ImGuiHelpers.ScaledVector2(100.0f, 23.0f)) && hotkeyHeld)
        {
            Result = ConfigurationResult.RemoveEntry;
        }
        if (!hotkeyHeld) ImGui.PopStyleVar();
        if (ImGui.IsItemHovered() && !hotkeyHeld)
        {
            ImGui.SetTooltip("Hold Shift + Alt while clicking to delete this rule");
        }
        ImGuiHelpers.ScaledDummy(5.0f);
    }

    private void DrawBody()
    {
        if (ImGui.BeginChild("##ContentsFrame", new Vector2(0.0f, -35.0f), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            if (ImGui.BeginTabBar("##RuleConfigTabBar"))
            {
                rule.DrawConfig();

                ImGui.EndTabBar();
            }
        }
        ImGui.EndChild();
    }

    private void DrawFooter()
    {
        if (ImGui.BeginTable("##SaveAndCloseTable", 2, ImGuiTableFlags.SizingStretchSame))
        {
            ImGui.TableNextColumn();
            ImGui.TextColored(KnownColor.Gray.AsVector4(), rule.Id);

            ImGui.TableNextColumn();
            ImGui.SetCursorPos(new Vector2(ImGui.GetContentRegionMax().X - 93.0f * 2.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().ItemSpacing.X - ImGui.GetStyle().FramePadding.X, ImGui.GetCursorPos().Y));
            if (ImGui.Button("Save##SaveButton", ImGuiHelpers.ScaledVector2(93.0f, 23.0f)))
            {
                SortaKindaSystem.SortController.SaveConfig();
            }

            ImGui.SameLine();
            if (ImGui.Button("Save & Close##SaveAndCloseButton", ImGuiHelpers.ScaledVector2(93.0f, 23.0f)))
            {
                Result = ConfigurationResult.SaveAndClose;
            }

            ImGui.EndTable();
        }
    }

    public override void OnClose()
    {
        Result = ConfigurationResult.SaveAndClose;
    }
}