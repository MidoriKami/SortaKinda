using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.System;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Abstracts;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.System;

public unsafe class SortController : IDisposable
{
    private static RuleConfig _ruleConfig = new();
    public static SortingRule? SelectedRule;
    private int selectedIndex;
    static Random _randomGenerator = new();

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
                Color = new Vector4(
                    _randomGenerator.Next(0, 255) / 255.0f, 
                    _randomGenerator.Next(0, 255) / 255.0f,
                    _randomGenerator.Next(0, 255) / 255.0f,
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
    
    private static IEnumerable<SortingRule> GetAllRules(params InventoryGrid[] grids) => 
        (from grid in grids 
            from slot in grid.InventorySlots 
            select slot.Rule)
        .ToHashSet()
        .Reverse();
    
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
        
        // Get all Sorting Rules, order by descending, so higher priority rules get the items in the end
        var sortingRules = GetAllRules(grids).ToList();
        
        // Get All ItemSlots that match this rule
        foreach (var rule in sortingRules)
        {
            if (rule.Id is "Default") continue;
            
            // Get all items this rule applies to
            var itemSlotsForRule = grids
                .SelectMany(grid => grid.InventorySlots)
                .Where(slot => rule.Filter.AllowedItemTypes.Contains(slot.LuminaData?.ItemUICategory.Row ?? uint.MaxValue))
                .ToList();
            
            // Get all target slots this rule applies to
            var targetSlotsForRule = grids
                .SelectMany(grid => grid.InventorySlots)
                .Where(slot => slot.Rule.Equals(rule))
                .ToList();

            if (SortaKindaSystem.SystemConfig.FillFromBottom)
                targetSlotsForRule.Reverse();
            
            // Order these slots, and limit to only the number of target locations possible
            var sourceSlotsOrdered = rule.Order
                .OrderItems(itemSlotsForRule)
                .Take(targetSlotsForRule.Count)
                .ToList();
            
            SortItems(targetSlotsForRule, sourceSlotsOrdered);
        }
        
        CleanupInventory(grids);

        foreach (var rule in sortingRules)
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
        // Get all Sorting Rules for these grids
        var sortingRules = GetAllRules(grids);

        // For each rule
        foreach (var rule in sortingRules)
        {
            if(rule.Id is "Default") continue;
            
            // Get all InventorySlot's for this rule, where the item doesn't match the filter
            var inventorySlotsForRule = grids
                .SelectMany(grid => grid.InventorySlots)
                .Where(slot => slot.Rule.Equals(rule) && slot.HasItem)
                .Where(slot => !rule.Filter.AllowedItemTypes.Contains(slot.LuminaData?.ItemUICategory.Row ?? uint.MaxValue));
            
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