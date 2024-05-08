using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace SortaKinda.Interfaces;

public interface IOneColumnRuleConfigurationTab : IRuleConfigurationTab {
    string FirstLabel { get; }

    void IRuleConfigurationTab.DrawConfigurationTab() {
        using var table = ImRaii.Table("##RuleConfigTable", 1, ImGuiTableFlags.SizingStretchSame, ImGui.GetContentRegionAvail());
       
        ImGui.TableNextColumn();
        ImGui.TextUnformatted(FirstLabel);
        ImGui.Separator();

        ImGui.TableNextColumn();
        DrawContents();
    }

    void DrawContents();
}