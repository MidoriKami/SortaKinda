using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class SellPriceOrdering : OrderingRuleBase {
	public override string Label
		=> "Vendor Sell Price";

	protected override string NotReversedLabel
		=> "High";

	protected override string ReversedLabel
		=> "Low";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> left->SellPrice.CompareTo(right->SellPrice);
}