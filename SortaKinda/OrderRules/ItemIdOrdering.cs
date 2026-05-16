using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class ItemIdOrdering : OrderingRuleBase {
	public override string Label
		=> "Item Id";

	public override string ButtonLabel
		=> IsReversed ? "High → Low" : "Low → High";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> left->ItemId.CompareTo(right->ItemId);
}