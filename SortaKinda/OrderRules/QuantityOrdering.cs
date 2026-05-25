using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class QuantityOrdering : OrderingRuleBase {
	public override string Label
		=> "Quantity";

	protected override string NotReversedLabel
		=> "Low";

	protected override string ReversedLabel
		=> "High";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> left->Quantity.CompareTo(right->Quantity);
}
