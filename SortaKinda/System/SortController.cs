using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.System;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Abstracts;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.System;

public unsafe class SortController : IDisposable
{
    private static RuleConfig _ruleConfig = new();
    public static SortingRule SelectedRule => _ruleConfig.SortingRules[_selectedIndex];
    private static int _selectedIndex;
    private static readonly Random RandomGenerator = new();

    public static SortingRule GetRule(string id)
        => _ruleConfig.SortingRules.Where(rule => rule.Id == id).FirstOrDefault() ?? _ruleConfig.SortingRules[0];
    
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

        if (ImGui.BeginChild("##RulesChild", new Vector2(0.0f, -40.0f), false, ImGuiWindowFlags.NoScrollbar))
        {
            foreach(var index in Enumerable.Range(0, _ruleConfig.SortingRules.Count))
            {
                var rule = _ruleConfig.SortingRules[index];
            
                if (ImGuiComponents.IconButton($"##DownButton{rule.Id}", FontAwesomeIcon.ArrowDown))
                {
                    if (_ruleConfig.SortingRules.Count > 1)
                    {
                        _ruleConfig.SortingRules.Remove(rule);
                        _ruleConfig.SortingRules.Insert(index + 1, rule);
                        SaveConfig();
                    }
                }

                ImGui.SameLine();
                if (ImGuiComponents.IconButton($"##UpButton{rule.Id}", FontAwesomeIcon.ArrowUp))
                {
                    if (_ruleConfig.SortingRules.Count > 1)
                    {
                        _ruleConfig.SortingRules.Remove(rule);
                        _ruleConfig.SortingRules.Insert(index - 1, rule);
                        SaveConfig();
                    }
                }
            
                ImGui.SameLine();
                ImGui.RadioButton($"##Selected{rule.Id}", ref _selectedIndex, index);

                ImGui.SameLine();
                rule.DrawHeader();

                ImGui.BeginDisabled(rule.Id is "Default");
                ImGui.SameLine();
                ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X - 25.0f });
                if (ImGuiComponents.IconButton($"##EditButton{rule.Id}", FontAwesomeIcon.Cog))
                {
                    SortaKindaSystem.WindowController.AddNewWindow(rule);
                }
                ImGui.EndDisabled();
            
                switch (SortaKindaSystem.WindowController.UpdateWindow(rule))
                {
                    case ConfigurationResult.SaveAndClose:
                        SaveConfig();
                        break;
                
                    case ConfigurationResult.RemoveEntry:
                        removalRule = rule;
                        break;
                }
            }

            if (removalRule is { } toRemove)
            {
                _ruleConfig.SortingRules.Remove(toRemove);
                SaveConfig();
            }
        }
        ImGui.EndChild();
        
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { Y = ImGui.GetContentRegionMax().Y - 32.0f });
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##NewRule", new Vector2(ImGui.GetContentRegionAvail().X, 23.0f)))
        {
            var newId = Guid.NewGuid().ToString("N");
            var newRule = new SortingRule
            {
                Id = newId,
                Color = new Vector4(
                    RandomGenerator.Next(0, 255) / 255.0f, 
                    RandomGenerator.Next(0, 255) / 255.0f,
                    RandomGenerator.Next(0, 255) / 255.0f,
                    1.0f),
                Name = "New Filter",
            };
            
            _ruleConfig.SortingRules.Add(newRule);
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
            
            SaveConfig();
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

    private static void SortItems(IReadOnlyList<InventorySlot> targetSlots, IReadOnlyList<InventorySlot> sourceSlots)
    {
        foreach (var index in Enumerable.Range(0, Math.Min(targetSlots.Count, sourceSlots.Count)))
        {
            SwapItem(targetSlots[index], sourceSlots[index]);
        }
    }

    private static void SwapItem(InventorySlot target, InventorySlot source)
    {
        var slotData = target.ItemOrderData;
        var itemData = source.ItemOrderData;
    
        (*slotData.Value, *itemData.Value) = (*itemData.Value, *slotData.Value);
    }
    
    public static void SortInventory(InventoryType type, params InventoryGrid[] grids)
    {
        PluginLog.Debug($"Sorting Inventory: {type}");
        
        // Get All ItemSlots that match this rule
        foreach (var rule in _ruleConfig.SortingRules)
        {
            if (rule.Id is "Default") continue;
            
            // Get all items this rule applies to
            var itemSlotsForRule = grids
                .SelectMany(grid => grid.InventorySlots)
                .Where(slot => rule.Filter.IsItemSlotAllowed(slot))
                .ToList();
            
            // Get all target slots this rule applies to
            var targetSlotsForRule = grids
                .SelectMany(grid => grid.InventorySlots)
                .Where(slot => slot.Rule.Equals(rule))
                .ToList();
            
            SortItems(targetSlotsForRule, itemSlotsForRule);
        }
        
        CleanupInventory(grids);

        foreach (var rule in _ruleConfig.SortingRules)
        {
            if (rule.Id is "Default") continue;

            // Get all target slots this rule applies to
            var targetSlotsForRule = grids
                .SelectMany(grid => grid.InventorySlots)
                .Where(slot => slot.Rule.Equals(rule))
                .ToList();
            
            if (SortaKindaSystem.SystemConfig.FillFromBottom)
                targetSlotsForRule.Reverse();

            ReorderItems(rule, targetSlotsForRule);
        }
    }

    private static void ReorderItems(SortingRule rule, IReadOnlyList<InventorySlot> items)
    {
        if (rule.Id is "Default") return;
        
        foreach (var _ in items)
        {
            foreach (var index in Enumerable.Range(0, items.Count - 1))
            {
                if (rule.Order.Compare(items[index], items[index + 1]))
                {
                    SwapItem(items[index], items[index + 1]);
                }
            }
        }
    }

    private static void CleanupInventory(params InventoryGrid[] grids)
    {
        // For each rule
        foreach (var rule in _ruleConfig.SortingRules)
        {
            if(rule.Id is "Default") continue;
            
            // Get all InventorySlot's for this rule, where the item doesn't match the filter
            var inventorySlotsForRule = grids
                .SelectMany(grid => grid.InventorySlots)
                .Where(slot => slot.Rule.Equals(rule) && slot.HasItem)
                .Where(slot => !rule.Filter.IsItemSlotAllowed(slot));
            
            // Get all empty unsorted InventorySlots
            var emptyInventorySlots = grids
                .SelectMany(grid => grid.InventorySlots)
                .Where(slot => slot.Rule.Id is "Default" && !slot.HasItem);

            if (SortaKindaSystem.SystemConfig.FillFromBottom)
                emptyInventorySlots = emptyInventorySlots.Reverse();
            
            // Perform the Sort
            SortItems(emptyInventorySlots.ToList(), inventorySlotsForRule.ToList());
        }
    }
}