using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class QualityOrdering : OrderingRuleBase {
	public override string Label
		=> "Quality";

	protected override string NotReversedLabel
		=> "NQ";

	protected override string ReversedLabel
		=> "HQ";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> left->IsHighQuality().CompareTo(right->IsHighQuality());
}
