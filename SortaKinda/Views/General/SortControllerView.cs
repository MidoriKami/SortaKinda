using System.Linq;
using Dalamud.Interface;
using ImGuiNET;
using Newtonsoft.Json;
using SortaKinda.Interfaces;
using SortaKinda.Models;

namespace SortaKinda.Views.SortControllerViews;

public class SortControllerView
{
    private readonly ISortController sortController;

    public SortingRuleListView ListView;

    public SortControllerView(ISortController sortingController)
    {
        sortController = sortingController;
        ListView = new SortingRuleListView(sortingController, sortingController.Rules);
    }
    
    public void Draw()
    {
        DrawHeader();
        DrawRules();
    }

    private void DrawHeader()
    {
        var importExportButtonSize = ImGuiHelpers.ScaledVector2(23.0f, 23.0f);
        var sortButtonSize = ImGuiHelpers.ScaledVector2(100.0f, 23.0f);
        
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3.0f * ImGuiHelpers.GlobalScale);
        ImGui.TextUnformatted("Sorting Rules");

        ImGui.SameLine(ImGui.GetContentRegionAvail().X - importExportButtonSize.X * 2.0f - sortButtonSize.X - ImGui.GetStyle().ItemSpacing.X * 2.0f);
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Clipboard.ToIconString()}##ImportButton", importExportButtonSize))
        {
            ImportRules();
        }
        ImGui.PopFont();
        if(ImGui.IsItemHovered()) ImGui.SetTooltip("Import rules from clipboard");
        
        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.ExternalLinkAlt.ToIconString()}##ImportButton", importExportButtonSize))
        {
            ExportRules();
        }
        ImGui.PopFont();
        if(ImGui.IsItemHovered()) ImGui.SetTooltip("Export rules to clipboard");

        ImGui.SameLine();
        if (ImGui.Button("Sort All", sortButtonSize))
        {
            sortController.SortAllInventories();
        }
        
        ImGui.Separator();
    }
    
    private void ImportRules()
    {
        if (JsonConvert.DeserializeObject<SortingRule[]>(ImGui.GetClipboardText()) is { } rules)
        {
            foreach (var rule in rules)
            {
                if (!sortController.Rules.Any(existingRule => existingRule.Id == rule.Id))
                {
                    sortController.Rules.Add(new SortingRule
                    {
                        Id = rule.Id,
                        Color = rule.Color,
                        Index = sortController.Rules.Count,
                        Name = rule.Name
                    });
                }
            }
            
            sortController.SaveConfig();
        }
    }
    
    private void ExportRules()
    {
        var rules = sortController.Rules.ToArray()[1..];
        
        var jsonString = JsonConvert.SerializeObject(rules);
        ImGui.SetClipboardText(jsonString);
    }

    private void DrawRules()
    {
        ListView.Draw();
    }
}