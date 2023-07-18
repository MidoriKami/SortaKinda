using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.System;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.System;

public class SortController : IDisposable
{
    private static RuleConfig _ruleConfig = new();
    public static SortingRule? SelectedRule;
    private int selectedIndex;

    public void DrawConfig()
    {
        SortingRule? removalRule = null;
        
        ImGui.TextUnformatted("Sorting Rules");
        ImGuiComponents.HelpMarker("Select a Rule then Left Click on a inventory slot to apply that rule\nRight click on an inventory slot to clear that slots rule");
        ImGui.SameLine();
        var region = ImGui.GetContentRegionMax();
        ImGui.SetCursorPos(region with { X = region.X - 50.0f, Y = ImGui.GetCursorPos().Y});
        if (ImGui.Button("Sort All"))
        {
            SortaKindaSystem.ModuleController.SortAll();
        }
        
        ImGui.Separator();
        
        foreach(var index in Enumerable.Range(0, _ruleConfig.SortingRules.Count))
        {
            var rule = _ruleConfig.SortingRules[index];
            
            if (ImGuiComponents.IconButton($"##DownButton{rule.Id}", FontAwesomeIcon.ArrowDown))
            {
                if (_ruleConfig.SortingRules.Count > 1)
                {
                    _ruleConfig.SortingRules.Remove(rule);
                    _ruleConfig.SortingRules.Insert(index + 1, rule);
                }
            }

            ImGui.SameLine();
            if (ImGuiComponents.IconButton($"##UpButton{rule.Id}", FontAwesomeIcon.ArrowUp))
            {
                if (_ruleConfig.SortingRules.Count > 1)
                {
                    _ruleConfig.SortingRules.Remove(rule);
                    _ruleConfig.SortingRules.Insert(index - 1, rule);
                }
            }
            
            ImGui.SameLine();
            if (ImGui.RadioButton($"##Selected{rule.Id}", ref selectedIndex, index))
            {
                SelectedRule = rule;
                selectedIndex = index;
            }
                
            ImGui.SameLine();
            rule.DrawHeader();

            ImGui.SameLine();
            ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X - 25.0f });
            if (ImGuiComponents.IconButton($"##EditButton{rule.Id}", FontAwesomeIcon.Cog))
            {
                SortaKindaSystem.WindowController.AddNewWindow(rule);
            }
            
            switch (SortaKindaSystem.WindowController.UpdateWindow(rule))
            {
                case ConfigurationResult.SaveAndClose:
                    SaveConfig();
                    break;
                
                case ConfigurationResult.RemoveEntry when rule.Id is not "Default":
                    removalRule = rule;
                    break;
            }
        }

        if (removalRule is { } toRemove)
        {
            _ruleConfig.SortingRules.Remove(toRemove);
            SaveConfig();
        }
        
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { Y = ImGui.GetContentRegionMax().Y - 32.0f });
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##NewRule", new Vector2(ImGui.GetContentRegionAvail().X, 23.0f)))
        {
            var newId = Guid.NewGuid().ToString("N");
            var newRule = new SortingRule
            {
                Id = newId,
                Color = KnownColor.Aqua.AsVector4(),
                Name = "New Filter",
            };
            
            _ruleConfig.SortingRules.Add(newRule);
            
            ImGui.OpenPopup($"{newRule.Id}Config");

            SaveConfig();
        }
        ImGui.PopFont();
    }
    
    public void Load()
    {
        _ruleConfig = LoadConfig();

        if (_ruleConfig.SortingRules.Count is 0)
        {
            _ruleConfig.SortingRules.Add(new SortingRule {
                Name = "Unsorted",
                Color = KnownColor.White.AsVector4(),
                Filter = new SortingFilter(),
                Id = "Default",
                Order = new SortingOrder
                {
                    Direction = SortOrderDirection.Ascending,
                    Mode = SortOrderMode.Alphabetically
                },
            });
        }
    }

    public void Unload()
    {
        
    }
    
    public void Dispose()
    {
        
    }
    
    private RuleConfig LoadConfig() => FileController.LoadFile<RuleConfig>("SortingRules.config.json", _ruleConfig);
    public void SaveConfig() => FileController.SaveFile("SortingRules.config.json", _ruleConfig.GetType(), _ruleConfig);
}