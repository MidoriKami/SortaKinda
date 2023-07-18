using System.Numerics;
using ImGuiNET;
using KamiLib.Interfaces;
using SortaKinda.System;

namespace SortaKinda.Abstracts;

public interface IInventoryConfigurationTab : ITabItem
{
    private const float ConfigurationSize = 0.40f;
    
    int TabOrder { get; }
    
    void ITabItem.Draw()
    {
        if (ImGui.BeginTable("##SortaKindaInventoryConfigTable", 2, ImGuiTableFlags.None, new Vector2(0, -1)))
        {
            ImGui.TableSetupColumn("##RulesColumn", ImGuiTableColumnFlags.WidthStretch, 47.5f);
            ImGui.TableSetupColumn("##InventoryColumn", ImGuiTableColumnFlags.WidthStretch, 52.5f);

            ImGui.TableNextColumn();
            if (ImGui.BeginChild("##ConfigChild", new Vector2(0, 0), false))
            {
                DrawRuleConfiguration();
            }
            ImGui.EndChild();
            
            ImGui.TableNextColumn();
            if (ImGui.BeginChild("##InventoryChild", new Vector2(0, 0), false, ImGuiWindowFlags.NoMove))
            {
                DrawInventory();
            }
            ImGui.EndChild();
            
            ImGui.EndTable();
        }
    }

    void DrawRuleConfiguration() => SortaKindaSystem.SortController.DrawConfig();
    
    void DrawInventory();
}