using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using KamiLib;
using SortaKinda.Models;
using SortaKinda.Models.Enum;
using SortaKinda.Views.Windows;

namespace SortaKinda.System;

public class WindowController : IDisposable
{
    private readonly Dictionary<string, RuleConfigurationWindow> temporaryWindows = new();

    public WindowController()
    {
        
    }

    public void Dispose()
    {
        
    }

    public void AddNewWindow(SortingRule rule)
    {
        if (!temporaryWindows.ContainsKey(rule.Id))
        {
            var newRuleWindow = new RuleConfigurationWindow(rule)
            {
                Position = ImGui.GetMousePos(),
                PositionCondition = ImGuiCond.Appearing,
                IsOpen = true
            };

            KamiCommon.WindowManager.AddWindow(newRuleWindow);
            temporaryWindows.Add(rule.Id, newRuleWindow);
        }
    }
    
    public ConfigurationResult UpdateWindow(SortingRule rule)
    {
        if (temporaryWindows.ContainsKey(rule.Id))
        {
            if (temporaryWindows[rule.Id] is { Result: var result } window && result != ConfigurationResult.None)
            {
                KamiCommon.WindowManager.RemoveWindow(window);
                temporaryWindows.Remove(rule.Id);
                return result;
            }
        }

        return ConfigurationResult.None;
    }
}