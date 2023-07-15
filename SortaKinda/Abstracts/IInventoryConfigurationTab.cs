using System.Numerics;
using ImGuiNET;
using KamiLib.Interfaces;

namespace SortaKinda.Abstracts;

public interface IInventoryConfigurationTab : ITabItem
{
    private const float ConfigurationSize = 0.40f;
    
    void ITabItem.Draw()
    {
        if (ImGui.BeginTable("##SortaKindaInventoryConfigTable", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.NoSavedSettings, new Vector2(0, -1)))
        {
            ImGui.TableSetupColumn("##RulesColumn", ImGuiTableColumnFlags.WidthStretch, 40.0f);
            ImGui.TableSetupColumn("##InventoryColumn", ImGuiTableColumnFlags.WidthStretch, 60.0f);

            ImGui.TableNextColumn();
            if (ImGui.BeginChild("##ConfigChild", new Vector2(0, 0)))
            {
                DrawRuleConfiguration();
            }
            ImGui.EndChild();
            
            ImGui.TableNextColumn();
            if (ImGui.BeginChild("##InventoryChild", new Vector2(0, 0)))
            {
                DrawInventory();
            }
            ImGui.EndChild();
            
            ImGui.EndTable();
        }
    }

    void DrawRuleConfiguration();
    
    void DrawInventory();
}