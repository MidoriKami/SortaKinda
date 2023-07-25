using ImGuiNET;

namespace SortaKinda.Interfaces;

public interface IOneColumnRuleConfigurationTab : IRuleConfigurationTab
{
    string FirstLabel { get; }
    
    void IRuleConfigurationTab.DrawConfigurationTab()
    {
        if (ImGui.BeginTable("##RuleConfigTable", 1, ImGuiTableFlags.SizingStretchSame, ImGui.GetContentRegionAvail()))
        {
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(FirstLabel);
            ImGui.Separator();
    
            ImGui.TableNextColumn();
            DrawContents();
            
            ImGui.EndTable();
        }
    }

    void DrawContents();
}