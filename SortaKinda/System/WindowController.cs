using System.Collections.Generic;
using KamiLib;
using SortaKinda.Models;
using SortaKinda.Models.Enum;
using SortaKinda.Views.Windows;

namespace SortaKinda.System;

public class WindowController
{
    private readonly Dictionary<string, RuleConfigurationWindow> temporaryWindows = new();

    public void AddNewWindow(ISortingRule rule)
    {
        if (!temporaryWindows.ContainsKey(rule.Id))
        {
            var newRuleWindow = new RuleConfigurationWindow(rule)
            {
                IsOpen = true
            };

            KamiCommon.WindowManager.AddWindow(newRuleWindow);
            temporaryWindows.Add(rule.Id, newRuleWindow);
        }
    }

    public ConfigurationResult UpdateWindow(ISortingRule rule)
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