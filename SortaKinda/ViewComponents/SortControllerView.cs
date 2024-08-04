using System;
using System.Linq;
using System.Text.Json;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.Window;
using SortaKinda.Classes;
using SortaKinda.Controllers;
using SortaKinda.Modules;
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
            System.WindowManager.AddWindow(new TutorialWindow(), WindowFlags.OpenImmediately);
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

    private record ClipboardRules(SortingRule[] Rules, MainInventoryConfig MainInventory, ArmoryConfig Armory);
    
    private void ImportRules() {
        try {
            var decodedString = Convert.FromBase64String(ImGui.GetClipboardText());
            var uncompressed = Util.DecompressString(decodedString);

            if (uncompressed.IsNullOrEmpty()) {
                Service.ChatGui.PrintError("Tried to import sorting rules, but got nothing, try copying the code again.");
                return;
            }

            if (JsonSerializer.Deserialize<ClipboardRules>(uncompressed, SerializerOptions) is { } clipboardData) {
                if (clipboardData.Rules.Length is 0) {
                    Service.ChatGui.PrintError("Tried to import sorting rules, but got nothing, try copying the code again.");
                    return;
                }

                var addedCount = 0;
                foreach (var rule in clipboardData.Rules) {
                    if (sortingController.Rules.All(existingRule => existingRule.Id != rule.Id)) {
                        rule.Index = sortingController.Rules.Count;
                        sortingController.Rules.Add(rule);
                        addedCount++;
                    }
                }

                Service.ChatGui.Print($"Received {clipboardData.Rules.Length} sorting rules from clipboard. ", "Import");
                Service.ChatGui.Print($"Added {addedCount} new sorting rules.", "Import");
                sortingController.SaveConfig();

                var mainInventoryModule = System.ModuleController.GetModule<MainInventoryModule>();
                mainInventoryModule.Config = clipboardData.MainInventory;
                mainInventoryModule.Save();
                mainInventoryModule.LoadModule();
                
                var armoryInventoryModule = System.ModuleController.GetModule<ArmoryInventoryModule>();
                armoryInventoryModule.Config = clipboardData.Armory;
                armoryInventoryModule.Save();
                armoryInventoryModule.LoadModule();
            }
        }
        catch (Exception e) {
            Service.ChatGui.PrintError("Something went wrong trying to import rules, check you copied the code correctly.");
            Service.Log.Error(e, "Handled exception while importing rules.");
        }
    }

    private static readonly JsonSerializerOptions SerializerOptions = new() {
        IncludeFields = true,
    };
    
    private void ExportRules() {
        var data = new ClipboardRules(
            sortingController.Rules.ToArray()[1..],
            (MainInventoryConfig) System.ModuleController.GetModule<MainInventoryModule>().Config,
            (ArmoryConfig) System.ModuleController.GetModule<ArmoryInventoryModule>().Config);
        
        var jsonString = JsonSerializer.Serialize(data, SerializerOptions);
        
        var compressed = Util.CompressString(jsonString);
        ImGui.SetClipboardText(Convert.ToBase64String(compressed));

        Service.ChatGui.Print($"Exported {data.Rules.Length} rules to clipboard.", "Export");
    }

    private void DrawRules() 
        => listView.Draw();
}
