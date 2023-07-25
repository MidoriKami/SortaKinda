using ImGuiNET;
using KamiLib.Interfaces;
using SortaKinda.System;

namespace SortaKinda.Interfaces;

public interface IInventoryConfigurationTab : ITabItem
{
    void ITabItem.Draw()
    {
        if (ImGui.BeginTable("##SortaKindaInventoryConfigTable", 2, ImGuiTableFlags.SizingStretchSame))
        {
            ImGui.TableNextColumn();
            if (ImGui.BeginChild("##ConfigChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding))
            {
                DrawRuleConfiguration();
            }
            ImGui.EndChild();

            ImGui.TableNextColumn();
            if (ImGui.BeginChild("##InventoryChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding, false, ImGuiWindowFlags.NoMove))
            {
                DrawInventory();
            }
            ImGui.EndChild();

            ImGui.EndTable();
        }
    }

    void DrawRuleConfiguration() => SortaKindaController.SortController.Draw();

    void DrawInventory();
}