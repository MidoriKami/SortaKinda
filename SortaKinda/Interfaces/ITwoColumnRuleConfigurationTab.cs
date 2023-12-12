using ImGuiNET;

namespace SortaKinda.Interfaces;

public interface ITwoColumnRuleConfigurationTab : IRuleConfigurationTab {
    string FirstLabel { get; }
    string SecondLabel { get; }

    void IRuleConfigurationTab.DrawConfigurationTab() {
        if (ImGui.BeginTable("##RuleConfigTable", 2, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.BordersInnerV, ImGui.GetContentRegionAvail())) {
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(FirstLabel);
            ImGui.Separator();

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(SecondLabel);
            ImGui.Separator();

            ImGui.TableNextColumn();
            DrawLeftSideContents();

            ImGui.TableNextColumn();
            DrawRightSideContents();

            ImGui.EndTable();
        }
    }

    void DrawLeftSideContents();
    void DrawRightSideContents();
}