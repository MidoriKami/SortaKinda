using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DailyDuty.System;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using KamiLib.Utilities;
using SortaKinda.Interfaces;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.System;

public unsafe class SortController
{
    private static RuleConfig _ruleConfig = new();
    private static int _selectedIndex;
    private static readonly Random RandomGenerator = new();
    public static ISortingRule SelectedRule => _ruleConfig.SortingRules[_selectedIndex];

    public static ISortingRule GetRule(string id)
    {
        return _ruleConfig.SortingRules.Where(rule => rule.Id == id).FirstOrDefault() ?? _ruleConfig.SortingRules[0];
    }

    public void DrawConfig()
    {
        ISortingRule? removalRule = null;

        ImGui.TextUnformatted("Sorting Rules");
        ImGuiComponents.HelpMarker("Select a Rule then Left Click on a inventory slot to apply that rule\nRight click on an inventory slot to clear that slots rule");
        ImGui.SameLine();
        var region = ImGui.GetContentRegionMax();
        ImGui.SetCursorPos(region with { X = region.X - 50.0f, Y = ImGui.GetCursorPos().Y });
        if (ImGui.Button("Sort All"))
        {
            SortaKindaSystem.ModuleController.SortAll();
        }
        ImGui.Separator();

        if (ImGui.BeginChild("##RulesChild", new Vector2(0.0f, -40.0f), false, ImGuiWindowFlags.NoScrollbar))
        {
            foreach (var index in Enumerable.Range(0, _ruleConfig.SortingRules.Count))
            {
                removalRule = DrawRule(index);
            }

            if (removalRule is { } toRemove)
            {
                _ruleConfig.SortingRules.Remove(toRemove);
                SaveConfig();
            }
        }
        ImGui.EndChild();

        DrawAddNewRuleButton();
    }

    private ISortingRule? DrawRule(int index)
    {
        var rule = _ruleConfig.SortingRules[index];

        DrawRuleMoveArrows(index, rule);

        ImGui.SameLine();
        ImGui.RadioButton($"##Selected{rule.Id}", ref _selectedIndex, index);

        ImGui.SameLine();
        rule.DrawListEntry();

        return DrawRuleConfigurationButton(rule);
    }

    private void DrawRuleMoveArrows(int index, ISortingRule rule)
    {
        ImGui.BeginDisabled(index is 0 || index == _ruleConfig.SortingRules.Count - 1);
        if (ImGuiComponents.IconButton($"##DownButton{rule.Id}", FontAwesomeIcon.ArrowDown))
        {
            if (_ruleConfig.SortingRules.Count > 1)
            {
                _ruleConfig.SortingRules.Remove(rule);
                _ruleConfig.SortingRules.Insert(index + 1, rule);
                SaveConfig();
            }
        }
        ImGui.EndDisabled();

        ImGui.SameLine();

        ImGui.BeginDisabled(index is 1 or 0);
        if (ImGuiComponents.IconButton($"##UpButton{rule.Id}", FontAwesomeIcon.ArrowUp))
        {
            if (_ruleConfig.SortingRules.Count > 1)
            {
                _ruleConfig.SortingRules.Remove(rule);
                _ruleConfig.SortingRules.Insert(index - 1, rule);
                SaveConfig();
            }
        }
        ImGui.EndDisabled();
    }

    private ISortingRule? DrawRuleConfigurationButton(ISortingRule rule)
    {
        ISortingRule? removalRule = null;
        var region = ImGui.GetContentRegionAvail();

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
                SortaKindaSystem.ModuleController.SortAll();
                SaveConfig();
                break;

            case ConfigurationResult.RemoveEntry:
                removalRule = rule;
                break;
        }

        return removalRule;
    }

    private void DrawAddNewRuleButton()
    {
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
                Name = "New Filter"
            };

            _ruleConfig.SortingRules.Add(newRule);
            SortaKindaSystem.WindowController.AddNewWindow(newRule);
            SaveConfig();
        }
        ImGui.PopFont();
    }

    public void Load()
    {
        _ruleConfig = LoadConfig();

        if (_ruleConfig.SortingRules.Count is 0)
        {
            _ruleConfig.SortingRules.Add(new SortingRule
            {
                Name = "Unsorted",
                Color = KnownColor.White.AsVector4(),
                Filter = new SortingFilter(),
                Id = "Default",
                Order = new SortingOrder
                {
                    Direction = SortOrderDirection.Ascending,
                    Mode = SortOrderMode.Alphabetically
                }
            });

            SaveConfig();
        }
    }

    private RuleConfig LoadConfig()
    {
        return FileController.LoadFile<RuleConfig>("SortingRules.config.json", _ruleConfig);
    }
    
    public void SaveConfig()
    {
        FileController.SaveFile("SortingRules.config.json", _ruleConfig.GetType(), _ruleConfig);
    }

    private static void SortItems(IReadOnlyList<IInventorySlot> targetSlots, IReadOnlyList<IInventorySlot> sourceSlots)
    {
        foreach (var index in Enumerable.Range(0, Math.Min(targetSlots.Count, sourceSlots.Count)))
        {
            SwapItem(targetSlots[index], sourceSlots[index]);
        }
    }

    private static void SwapItem(IInventorySlot target, IInventorySlot source)
    {
        var slotData = target.ItemOrderData;
        var itemData = source.ItemOrderData;

        (*slotData.Value, *itemData.Value) = (*itemData.Value, *slotData.Value);
    }

    public static void SortInventory(InventoryType type, params IInventoryGrid[] grids)
    {
        Task.Run(() =>
        {
            PluginLog.Debug($"Sorting Inventory: {type}");

            // Get All ItemSlots that match this rule
            foreach (var rule in _ruleConfig.SortingRules)
            {
                if (rule.Id is "Default") continue;

                // Get all items this rule applies to, and aren't already in any of the slots for that rule
                var itemSlotsForRule = grids
                    .SelectMany(grid => grid.InventorySlots)
                    .Where(slot => !slot.Rule.Equals(rule))
                    .Where(slot => rule.Filter.IsItemSlotAllowed(slot))
                    .Order(rule.Order)
                    .ToList();

                // Get all target slots this rule applies to, that doesn't have an item that's supposed to be there
                var targetSlotsForRule = grids
                    .SelectMany(grid => grid.InventorySlots)
                    .Where(slot => slot.Rule.Equals(rule))
                    .Where(slot => !rule.Filter.IsItemSlotAllowed(slot))
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

                ReorderItems(rule, targetSlotsForRule);
            }
        });
    }

    private static void ReorderItems(ISortingRule rule, IReadOnlyList<IInventorySlot> items)
    {
        if (rule.Id is "Default") return;

        foreach (var _ in items)
        {
            foreach (var index in Enumerable.Range(0, items.Count - 1))
            {
                if (rule.Order.CompareSlots(items[index], items[index + 1]))
                {
                    SwapItem(items[index], items[index + 1]);
                }
            }
        }
    }

    private static void CleanupInventory(params IInventoryGrid[] grids)
    {
        // For each rule
        foreach (var rule in _ruleConfig.SortingRules)
        {
            if (rule.Id is "Default") continue;

            // Get all IInventorySlot's for this rule, where the item doesn't match the filter
            var inventorySlotsForRule = grids
                .SelectMany(grid => grid.InventorySlots)
                .Where(slot => slot.Rule.Equals(rule) && slot.HasItem)
                .Where(slot => !rule.Filter.IsItemSlotAllowed(slot));

            // Get all empty unsorted InventorySlots
            var emptyInventorySlots = grids
                .SelectMany(grid => grid.InventorySlots)
                .Where(slot => slot.Rule.Id is "Default" && !slot.HasItem);

            // Perform the Sort
            SortItems(emptyInventorySlots.ToList(), inventorySlotsForRule.ToList());
        }
    }
}