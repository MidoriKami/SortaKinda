using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class SellPriceOrdering : OrderingRuleBase {
	public override string Label
		=> "Vendor Sell Price";

	public override string ButtonLabel
		=> IsReversed ? "High → Low" : "Low → High";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> left->SellPrice.CompareTo(right->SellPrice);
}