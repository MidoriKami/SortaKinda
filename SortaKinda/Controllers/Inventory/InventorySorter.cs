using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.NativeUi;
using SortaKinda.Interfaces;

namespace SortaKinda.System;

public unsafe class InventorySorter {
    private static void SwapItems(IReadOnlyList<IInventorySlot> targetSlots, IReadOnlyList<IInventorySlot> sourceSlots) {
        foreach (var index in Enumerable.Range(0, Math.Min(targetSlots.Count, sourceSlots.Count))) {
            SwapItem(targetSlots[index], sourceSlots[index]);
        }
    }

    private static void SwapItem(IInventorySlot target, IInventorySlot source) {
        var slotData = target.ItemOrderEntry;
        var itemData = source.ItemOrderEntry;

        (*slotData, *itemData) = (*itemData, *slotData);
    }

    public static void SortInventory(InventoryType type, params IInventoryGrid[] grids) => Safety.ExecuteSafe(() => {
        var stopwatch = Stopwatch.StartNew();
        Service.Log.Debug($"Sorting Inventory: {type}");

        // Get all rules for this inventory for priority determinations
        var rulesForInventory = grids
            .SelectMany(grid => grid.Inventory)
            .Select(slots => slots.Rule)
            .ToHashSet();

        // Step 1: Put all items that belong into a category into a category
        MoveItemsIntoCategories(grids, rulesForInventory);

        // Step 2: Remove items that don't belong in categories
        RemoveItemsFromCategories(grids);

        // Step 3: Sort remaining items in categories
        SortCategories(grids);

        Service.Log.Debug($"Sorted {type} in {stopwatch.Elapsed.TotalMilliseconds}ms");
    }, $"Exception Caught During Sorting '{type}'");

    private static void MoveItemsIntoCategories(IInventoryGrid[] grids, IEnumerable<ISortingRule> rulesForInventory) {
        foreach (var rule in SortaKindaController.SortController.Rules) {
            if (rule.Id is SortController.DefaultId) continue;

            // Get all items this rule applies to, and aren't already in any of the slots for that rule
            var itemSlotsForRule = grids
                .SelectMany(grid => grid.Inventory)
                .Where(slot => !slot.Rule.Equals(rule))
                .Where(slot => rule.IsItemSlotAllowed(slot))
                .Where(slot => !rulesForInventory.Any(otherRules => otherRules.Index > rule.Index && otherRules.IsItemSlotAllowed(slot)))
                .Order(rule)
                .ToArray();

            // Get all target slots this rule applies to, that doesn't have an item that's supposed to be there
            var targetSlotsForRule = grids
                .SelectMany(grid => grid.Inventory)
                .Where(slot => slot.Rule.Equals(rule))
                .Where(slot => !rule.IsItemSlotAllowed(slot))
                .ToArray();

            SwapItems(targetSlotsForRule, itemSlotsForRule);
        }
    }

    private static void RemoveItemsFromCategories(IInventoryGrid[] grids) {
        foreach (var rule in SortaKindaController.SortController.Rules) {
            if (rule.Id is SortController.DefaultId) continue;

            // Get all IInventorySlot's for this rule, where the item doesn't match the filter
            var inventorySlotsForRule = grids
                .SelectMany(grid => grid.Inventory)
                .Where(slot => slot.Rule.Equals(rule) && slot.HasItem)
                .Where(slot => !rule.IsItemSlotAllowed(slot));

            // Get all empty unsorted InventorySlots
            var emptyInventorySlots = grids
                .SelectMany(grid => grid.Inventory)
                .Where(slot => slot.Rule.Id is SortController.DefaultId && !slot.HasItem);

            // Perform the Sort
            SwapItems(emptyInventorySlots.ToList(), inventorySlotsForRule.ToList());
        }
    }

    private static void SortCategories(IInventoryGrid[] grids) {
        foreach (var rule in SortaKindaController.SortController.Rules) {
            if (rule.Id is SortController.DefaultId) continue;

            // Get all target slots this rule applies to
            var targetSlotsForRule = grids
                .SelectMany(grid => grid.Inventory)
                .Where(slot => slot.Rule.Equals(rule))
                .ToList();

            foreach (var _ in targetSlotsForRule) {
                foreach (var index in Enumerable.Range(0, targetSlotsForRule.Count - 1)) {
                    if (rule.CompareSlots(targetSlotsForRule[index], targetSlotsForRule[index + 1])) {
                        SwapItem(targetSlotsForRule[index], targetSlotsForRule[index + 1]);
                    }
                }
            }
        }
    }
}