using System;
using System.Linq;
using System.Text.Json;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Classes;
using SortaKinda.Classes;
using SortaKinda.Controllers;
using SortaKinda.Windows;

namespace SortaKinda.ViewComponents;

public class SortControllerView(SortController sortingController) {
    private readonly SortingRuleListView listView = new(sortingController, sortingController.Rules);

    public void Draw() {
        DrawHeader();
        DrawRules();
    }

    private void DrawHeader() {
        var importExportButtonSize = ImGuiHelpers.ScaledVector2(23.0f, 23.0f);
        var sortButtonSize = ImGuiHelpers.ScaledVector2(100.0f, 23.0f);

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 3.0f * ImGuiHelpers.GlobalScale);
        ImGui.TextUnformatted("Sorting Rules");

        ImGui.SameLine(ImGui.GetContentRegionAvail().X - importExportButtonSize.X * 3.0f - sortButtonSize.X - ImGui.GetStyle().ItemSpacing.X * 3.0f);

        if (ImGuiTweaks.IconButtonWithSize(Service.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.Question, "HelpButton", importExportButtonSize, "Open Help Window")) {
            SortaKindaController.WindowManager.AddWindow(new TutorialWindow());
        }

        ImGui.SameLine();
        if (ImGuiTweaks.IconButtonWithSize(Service.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.Clipboard, "ImportButton", importExportButtonSize, "Import rules from clipboard")) {
            ImportRules();
        }
        
        ImGui.SameLine();
        if (ImGuiTweaks.IconButtonWithSize(Service.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.ExternalLinkAlt, "ExportButton", importExportButtonSize, "Export rules to clipboard")) {
            ExportRules();
        }

        ImGui.SameLine();
        if (ImGui.Button("Sort All", sortButtonSize)) {
            sortingController.SortAllInventories();
        }

        ImGui.Separator();
    }

    private void ImportRules() {
        try {
            var decodedString = Convert.FromBase64String(ImGui.GetClipboardText());
            var uncompressed = Util.DecompressString(decodedString);

            if (uncompressed.IsNullOrEmpty()) {
                Service.ChatGui.PrintError("Tried to import sorting rules, but got nothing, try copying the code again.");
                return;
            }

            if (JsonSerializer.Deserialize<SortingRule[]>(uncompressed) is { } rules) {
                if (rules.Length is 0) {
                    Service.ChatGui.PrintError("Tried to import sorting rules, but got nothing, try copying the code again.");
                    return;
                }

                var addedCount = 0;
                foreach (var rule in rules) {
                    if (!sortingController.Rules.Any(existingRule => existingRule.Id == rule.Id)) {
                        rule.Index = sortingController.Rules.Count;
                        sortingController.Rules.Add(rule);
                        addedCount++;
                    }
                }

                Service.ChatGui.Print($"Received {rules.Length} sorting rules from clipboard. ", "Import");
                Service.ChatGui.Print($"Added {addedCount} new sorting rules.", "Import");
                sortingController.SaveConfig();
            }
        }
        catch {
            Service.ChatGui.PrintError("Something went wrong trying to import rules, check you copied the code correctly.");
        }
    }

    private void ExportRules() {
        var rules = sortingController.Rules.ToArray()[1..];
        var jsonString = JsonSerializer.Serialize(rules);
        
        var compressed = Util.CompressString(jsonString);
        ImGui.SetClipboardText(Convert.ToBase64String(compressed));

        Service.ChatGui.Print($"Exported {rules.Length} rules to clipboard.", "Export");
    }

    private void DrawRules() 
        => listView.Draw();
}