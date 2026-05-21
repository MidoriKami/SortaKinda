using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class ItemIdOrdering : OrderingRuleBase {
	public override string Label
		=> "Item Id";

	protected override string NotReversedLabel
		=> "Low";

	protected override string ReversedLabel
		=> "High";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> right->ItemId.CompareTo(left->ItemId);
}