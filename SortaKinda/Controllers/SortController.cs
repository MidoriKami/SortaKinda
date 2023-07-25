using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KamiLib.Utilities;
using SortaKinda.Interfaces;
using SortaKinda.Models;
using SortaKinda.Models.Configuration;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.System;

public class SortController : ISortController
{
    public const string DefaultId = "Default";
    public int SelectedRuleIndex = 0;
    public SortingRuleConfig RuleConfig { get; set; } = new();
    public ISortingRule SelectedRule => RuleConfig.Rules[SelectedRuleIndex];
    public SortControllerView? View { get; set; }

    private static SortingRule DefaultRule => new()
    {
        Id = DefaultId,
        Name = "Unsorted",
        Index = 0,
        Color = KnownColor.White.AsVector4()
    };

    public List<SortingRule> Rules => RuleConfig.Rules;

    public void SortAllInventories()
    {
        SortaKindaController.ModuleController.Sort();
    }
    
    public void SaveConfig()
    {
        CharacterFileController.SaveFile("SortingRules.config.json", RuleConfig.GetType(), RuleConfig);
    }

    public void Load()
    {
        RuleConfig = new SortingRuleConfig();
        RuleConfig = LoadConfig();
        View = new SortControllerView(this);
        EnsureDefaultRule();
    }

    public void Draw()
    {
        View?.Draw();
    }

    public ISortingRule GetRule(string id)
    {
        return RuleConfig.Rules.FirstOrDefault(rule => rule.Id == id) ?? RuleConfig.Rules[0];
    }

    private void EnsureDefaultRule()
    {
        if (RuleConfig.Rules.Count is 0)
        {
            RuleConfig.Rules.Add(DefaultRule);
        }

        if (RuleConfig.Rules[0] is not { Id: DefaultId, Name: "Unsorted", Index: 0 })
        {
            RuleConfig.Rules[0] = DefaultRule;
        }
    }

    private SortingRuleConfig LoadConfig()
    {
        return CharacterFileController.LoadFile<SortingRuleConfig>("SortingRules.config.json", RuleConfig);
    }
}