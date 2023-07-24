using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.System;

namespace SortaKinda.Models.Inventory;

public unsafe class InventorySlot : IInventorySlot
{
    public bool HasItem => Item is not null;
    public Item? Item => LuminaCache<Item>.Instance.GetRow(InventoryController.GetItemForSlot(Type, Slot)->ItemID);

    public ItemOrderModuleSorterItemEntry* ItemOrderEntry => InventoryController.GetItemOrderData(Type, Slot);
    public ISortingRule Rule => SortaKindaPlugin.Controller.SortController.GetRule(Config.RuleId);
    public InventoryType Type { get; init; }
    public int Slot { get; init; }
    public SlotConfig Config { get; init; }

    public InventorySlot(InventoryType type, SlotConfig config, int index)
    {
        Type = type;
        Config = config;
        Slot = index;
    }
        
    public void OnLeftClick()
    {
        Config.RuleId = SortaKindaPlugin.Controller.SortController.SelectedRule.Id;
        Config.NeedsSaving = true;
    }
    
    public void OnRightClick()
    {
        Config.RuleId = SortController.DefaultId;
        Config.NeedsSaving = true;
    }
    
    public void OnDragCollision()
    {
        Config.RuleId = SortaKindaPlugin.Controller.SortController.SelectedRule.Id;
        Config.NeedsSaving = true;
    }
    
    public void OnHover()
    {
        Rule.DrawTooltip();
    }
}