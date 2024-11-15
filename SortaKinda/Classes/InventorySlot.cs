using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using SortaKinda.Controllers;

namespace SortaKinda.Classes;

public class SlotConfig {
    [JsonIgnore] public bool Dirty;

    public string RuleId { get; set; } = SortController.DefaultId;
}

public unsafe class InventorySlot(InventoryType type, SlotConfig config, int index) {

    private InventoryType Type { get; } = type;

    public SlotConfig Config { get; init; } = config;

    [MemberNotNullWhen(true, "ExdItem")] 
    public bool HasItem => InventoryItem->ItemId is not 0;
    
    public Item ExdItem => Service.DataManager.GetExcelSheet<Item>().GetRow(InventoryItem->ItemId);
    
    public InventoryItem* InventoryItem => InventoryController.GetItemForSlot(Type, Slot);
    
    public ItemOrderModuleSorterItemEntry* ItemOrderEntry => InventoryController.GetItemOrderData(Type, Slot);

    public SortingRule Rule {
        get {
            var sortControllerRule = System.SortController.GetRule(Config.RuleId);

            if (sortControllerRule.Id != Config.RuleId) {
                TryApplyRule(sortControllerRule.Id);
            }
            return sortControllerRule;
        }
    }

    public int Slot { get; init; } = index;

    public void OnLeftClick() => TryApplyRule(System.SortController.SelectedRule.Id);

    public void OnRightClick() => TryApplyRule(SortController.DefaultId);

    public void OnDragCollision() => TryApplyRule(System.SortController.SelectedRule.Id);

    public void OnHover() => Rule.ShowTooltip();

    private void TryApplyRule(string id) {
        if (Config.RuleId != id) {
            Config.RuleId = id;
            Config.Dirty = true;
        }
    }
}