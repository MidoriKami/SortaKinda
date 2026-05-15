using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class ItemIdOrderingRule : OrderingRuleBase {
	public override string Label => "Item Id";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> left->ItemId.CompareTo(right->ItemId);
}