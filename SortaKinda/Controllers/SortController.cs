using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KamiLib.Utilities;
using SortaKinda.Interfaces;
using SortaKinda.Models;
using SortaKinda.Models.Configuration;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.System;

public class SortController : IDisposable, ISortController
{
    private bool IsLoaded { get; set; }
    
    public SortingRuleConfig RuleConfig { get; set; } = new();
    public const string DefaultId = "Default";
    public List<SortingRule> Rules => RuleConfig.Rules;
    public int SelectedRuleIndex = 0;
    public ISortingRule SelectedRule => RuleConfig.Rules[SelectedRuleIndex];
    public SortControllerView? View { get; set; }

    public void Load()
    {
        RuleConfig = new SortingRuleConfig();
        RuleConfig = LoadConfig();
        View = new SortControllerView(this);
        EnsureDefaultRule();
        
        IsLoaded = true;
    }

    public void Update()
    {
        
    }
    
    public void Draw()
    {
        if (!IsLoaded) return;
        
        View?.Draw();
    }

    public void Unload()
    {
        IsLoaded = false;
    }

    public ISortingRule GetRule(string id)
        => RuleConfig.Rules.FirstOrDefault(rule => rule.Id == id) ?? RuleConfig.Rules[0];

    public void SortAllInventories() => SortaKindaController.ModuleController.Sort();

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
    
    private static SortingRule DefaultRule => new()
    {
        Id = DefaultId,
        Name = "Unsorted",
        Index = 0,
        Color = KnownColor.White.AsVector4()
    };
    
    private SortingRuleConfig LoadConfig() => CharacterFileController.LoadFile<SortingRuleConfig>("SortingRules.config.json", RuleConfig);
    public void SaveConfig() => CharacterFileController.SaveFile("SortingRules.config.json", RuleConfig.GetType(), RuleConfig);
    public void Dispose() => Unload();
}