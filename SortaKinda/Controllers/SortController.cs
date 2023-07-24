using System.Drawing;
using System.Linq;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.Views.SortControllerViews;

namespace SortaKinda.System;

public class SortController : ISortController
{
    public SortingRuleConfig RuleConfig { get; set; } = new();
    public const string DefaultId = "Default";
    private int SelectedRuleIndex = 0;
    public ISortingRule SelectedRule => RuleConfig.Rules[SelectedRuleIndex];

    private bool IsLoaded { get; set; }
    
    public void Draw()
    {
        if (!IsLoaded)
        {
            ImGui.TextColored(KnownColor.Red.AsVector4(), "SortController is not Loaded.");
            return;
        }
        
        SortControllerView.Draw(this);
    }

    public ISortingRule GetRule(string id)
        => RuleConfig.Rules.FirstOrDefault(rule => rule.Id == id) ?? RuleConfig.Rules[0];
    
    public void Load()
    {
        RuleConfig = LoadConfig();
        IsLoaded = true;
    }

    public void Unload()
    {
        IsLoaded = false;
    }

    public void Update()
    {
        
    }
    
    private SortingRuleConfig LoadConfig() => CharacterFileController.LoadFile<SortingRuleConfig>("SortingRules.config.json", RuleConfig);

    public void SaveConfig() => CharacterFileController.SaveFile("SortingRules.config.json", RuleConfig.GetType(), RuleConfig);
}