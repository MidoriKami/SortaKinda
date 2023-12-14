using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using KamiLib.Game;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.Models.Configuration;
using SortaKinda.System;

namespace SortaKinda.Models.Inventory;

public unsafe class InventorySlot : IInventorySlot {
    public InventorySlot(InventoryType type, SlotConfig config, int index) {
        Type = type;
        Config = config;
        Slot = index;
    }

    private InventoryType Type { get; }
    public SlotConfig Config { get; init; }
    public bool HasItem => InventoryItem->ItemID is not 0;
    public Item? ExdItem => LuminaCache<Item>.Instance.GetRow(InventoryItem->ItemID);
    public InventoryItem* InventoryItem => InventoryController.GetItemForSlot(Type, Slot);
    public ItemOrderModuleSorterItemEntry* ItemOrderEntry => InventoryController.GetItemOrderData(Type, Slot);

    public ISortingRule Rule {
        get {
            var sortControllerRule = SortaKindaController.SortController.GetRule(Config.RuleId);

            if (sortControllerRule.Id != Config.RuleId) {
                TryApplyRule(sortControllerRule.Id);
            }
            return sortControllerRule;
        }
    }

    public int Slot { get; init; }

    public void OnLeftClick() => TryApplyRule(SortaKindaController.SortController.SelectedRule.Id);

    public void OnRightClick() => TryApplyRule(SortController.DefaultId);

    public void OnDragCollision() => TryApplyRule(SortaKindaController.SortController.SelectedRule.Id);

    public void OnHover() => Rule.ShowTooltip();

    private void TryApplyRule(string id) {
        if (Config.RuleId != id) {
            Config.RuleId = id;
            Config.Dirty = true;
        }
    }
}