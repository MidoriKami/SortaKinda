using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Abstracts;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.System.Modules;

public unsafe class MainInventoryModule : InventoryModuleBase
{
    public override ModuleName ModuleName { get; protected set; } = ModuleName.MainInventory;
    public override IModuleConfig ModuleConfig { get; set; } = new MainModuleConfig();

    private InventoryGrid inventory1 = null!;
    private InventoryGrid inventory2 = null!;
    private InventoryGrid inventory3 = null!;
    private InventoryGrid inventory4 = null!;
    
    protected override void LoadModule()
    {
        ModuleConfig.Configurations ??= new Dictionary<InventoryType, InventoryConfig>
        {
            { InventoryType.Inventory1, new InventoryConfig(InventoryType.Inventory1) },
            { InventoryType.Inventory2, new InventoryConfig(InventoryType.Inventory2) },
            { InventoryType.Inventory3, new InventoryConfig(InventoryType.Inventory3) },
            { InventoryType.Inventory4, new InventoryConfig(InventoryType.Inventory4) },
        };

        inventory1 = new InventoryGrid(InventoryType.Inventory1, this);
        inventory2 = new InventoryGrid(InventoryType.Inventory2, this);
        inventory3 = new InventoryGrid(InventoryType.Inventory3, this);
        inventory4 = new InventoryGrid(InventoryType.Inventory4, this);
    }
    
    // This works, but probably not properly
    // public override void PerformSort()
    // {
    //     // InventoryController.GetItemOrderData(type) returns the combined inventory for inv[1-4]
    //     foreach (var slot in InventoryController.GetItemOrderData(InventoryType.Inventory1)->Span)
    //     {
    //         var item = InventoryManager.Instance()->GetInventoryContainer((InventoryType) slot.Value->Page)->GetInventorySlot(slot.Value->Slot);
    //         if (item is null) continue;
    //         
    //         var luminaItem = LuminaCache<Item>.Instance.GetRow(item->ItemID);
    //         if (luminaItem is null) continue;
    //
    //         PluginLog.Debug($"{luminaItem.Name} :: {luminaItem.ItemUICategory.Row}");
    //         
    //         foreach (var inventorySlot in inventory2.InventorySlots)
    //         {
    //             if (inventorySlot.Rule.Filter.AllowedItemTypes.Contains(luminaItem.ItemUICategory.Row))
    //             {
    //                 PluginLog.Debug($"Match: {inventorySlot.ItemOrderData->Page} {inventorySlot.ItemOrderData->Slot}");
    //
    //                 (*slot.Value, *inventorySlot.ItemOrderData) = (*inventorySlot.ItemOrderData, *slot.Value);
    //             }
    //         }
    //     }
    // }

    // Failed Attempt, we tried getting the items to sort with, instead of the slots.
    // public override void PerformSort()
    // {
    //     // Get all Sorting Rules
    //     var sortingRules = GetAllRules().ToHashSet();
    //
    //     // Get All items that match this rule
    //     foreach (var rule in sortingRules)
    //     {
    //         // Items we possess that match this rule
    //         var itemsForRule = GetItemsForRule(rule);
    //
    //         // Those same items ordered
    //         var orderedItems = rule.Order.OrderItems(itemsForRule);
    //
    //         // All of the possible slots we can put these items
    //         var slots = GetSlotsForRule(rule).ToList();
    //
    //         // The items we ordered, but limited to the number of slots we have
    //         var itemsThatFitInSlots = orderedItems.Take(slots.Count).ToList();
    //
    //         var slotIndex = 0;
    //         foreach (var item in itemsThatFitInSlots)
    //         {
    //             var slotData = slots[slotIndex].ItemOrderData;
    //         }
    //
    //         // PluginLog.Debug(string.Join("\n", orderedItems.Select(items => LuminaCache<Item>.Instance.GetRow(items.ItemID)?.Name.RawString)));
    //     }
    // }

    public override void PerformSort()
    {
        // Get all Sorting Rules, order by descending, so higher priority rules get the items in the end
        var sortingRules = GetAllRules()
            .ToHashSet()
            .Reverse();
        
        // Get All ItemSlots that match this rule
        foreach (var rule in sortingRules)
        {
            // Item slots for existing items that match this rule
            var itemSlotsForRule = GetItemSlotsForRule(rule);
            
            // Item slots for existing items that match this rule, but ordered
            var orderedItemSlots = rule.Order.OrderItems(itemSlotsForRule, InventoryType.Inventory1);
            
            // All of the possible slots we can put these items in
            var slots = GetSlotsForRule(rule).ToList();

            // Ordered items that fit in the desired slots
            var itemsThatFitInSlots = orderedItemSlots.Take(slots.Count).ToList();

            foreach (var index in Enumerable.Range(0, Math.Min(slots.Count, itemsThatFitInSlots.Count)))
            {
                var slotData = slots[index].ItemOrderData;
                var itemData = itemsThatFitInSlots[index];

                (*slotData, *itemData.Value) = (*itemData.Value, *slotData);
            }
        }
    }
    
    private IEnumerable<Pointer<ItemOrderModuleSorterItemEntry>> GetItemSlotsForRule(SortingRule rule)
    {
        var results = new List<Pointer<ItemOrderModuleSorterItemEntry>>();
        
        // We only need inventory1, since this gives us the entire combined inventory
        foreach (var itemSlot in InventoryController.GetItemOrderData(InventoryType.Inventory1)->Span)
        {
            var itemForSlot = InventoryController.GetItemForSlot(InventoryType.Inventory1, itemSlot);
            // var itemForSlot = InventoryManager.Instance()->GetInventoryContainer((InventoryType) itemSlot.Value->Page)->GetInventorySlot(itemSlot.Value->Slot);
            
            var luminaData = LuminaCache<Item>.Instance.GetRow(itemForSlot->ItemID);
            if (luminaData is null) continue;

            if (rule.Filter.AllowedItemTypes.Contains(luminaData.ItemUICategory.Row))
            {
                results.Add(itemSlot);
            }
        }

        return results;
    }

    private IEnumerable<SortingRule> GetAllRules()
    {
        foreach (var slot in inventory1.InventorySlots) yield return slot.Rule;
        foreach (var slot in inventory2.InventorySlots) yield return slot.Rule;
        foreach (var slot in inventory3.InventorySlots) yield return slot.Rule;
        foreach (var slot in inventory4.InventorySlots) yield return slot.Rule;
    }

    private IEnumerable<InventoryItem> GetItemsForRule(SortingRule rule)
        => GetItemsForRuleAndInventory(rule, InventoryType.Inventory1)
            .Concat(GetItemsForRuleAndInventory(rule, InventoryType.Inventory2))
            .Concat(GetItemsForRuleAndInventory(rule, InventoryType.Inventory3))
            .Concat(GetItemsForRuleAndInventory(rule, InventoryType.Inventory4));


    private IEnumerable<InventoryItem> GetItemsForRuleAndInventory(SortingRule rule, InventoryType type)
    {
        var results = new List<InventoryItem>();
        
        foreach (var item in GetInventorySpan(type))
        {
            var luminaData = LuminaCache<Item>.Instance.GetRow(item.ItemID);
            if (luminaData is null) continue;

            if (rule.Filter.AllowedItemTypes.Contains(luminaData.ItemUICategory.Row))
            {
                results.Add(item);
            }
        }

        return results;
    }

    private IEnumerable<InventorySlot> GetSlotsForRule(SortingRule rule)
        => GetSlotsForRule(rule, inventory1)
            .Concat(GetSlotsForRule(rule, inventory2))
            .Concat(GetSlotsForRule(rule, inventory3))
            .Concat(GetSlotsForRule(rule, inventory4));

    private IEnumerable<InventorySlot> GetSlotsForRule(SortingRule rule, InventoryGrid grid)
        => grid.InventorySlots.Where(slot => slot.Rule.Equals(rule));
    
    private Span<InventoryItem> GetInventorySpan(InventoryType inventory) 
        => new(InventoryManager.Instance()->GetInventoryContainer(inventory)->Items, (int) InventoryManager.Instance()->GetInventoryContainer(inventory)->Size);
    
    public override void DrawInventoryGrid()
    {
        var region = ImGui.GetContentRegionAvail();

        var firstPosition = new Vector2(0.0f, 0.0f);
        var secondPosition = new Vector2(region.X / 2.0f, 0.0f);
        var thirdPosition = new Vector2(0.0f, region.Y / 2.0f);
        var fourthPosition = new Vector2(region.X / 2.0f, region.Y / 2.0f);
        
        inventory1.Draw(firstPosition);
        inventory2.Draw(secondPosition);
        inventory3.Draw(thirdPosition);
        inventory4.Draw(fourthPosition);
    }


    // [SingleTierCommandHandler("test", "sort")]
    // private void SortFunction()
    // {
    //     var correctType = (StdVector<Pointer<ItemOrderModuleSorterItemEntry>>*) &UIModule.Instance()->GetItemOrderModule()->InventorySorter->Items;
    //
    //     (correctType->First[1].Value->Slot, correctType->First[0].Value->Slot) = (correctType->First[0].Value->Slot, correctType->First[1].Value->Slot);
    // }
}