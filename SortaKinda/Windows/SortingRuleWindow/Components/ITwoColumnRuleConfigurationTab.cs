using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace SortaKinda.Windows.SortingRuleWindow.Components;

public interface ITwoColumnRuleConfigurationTab : IRuleConfigurationTab {
    string FirstLabel { get; }
    
    string SecondLabel { get; }

    void IRuleConfigurationTab.DrawConfigurationTab() {
        using var table = ImRaii.Table("##RuleConfigTable", 2, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.BordersInnerV, ImGui.GetContentRegionAvail());
        if (!table) return;
        
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
    }

    void DrawLeftSideContents();
    
    void DrawRightSideContents();
}