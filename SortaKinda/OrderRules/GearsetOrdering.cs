using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class GearsetOrdering : OrderingRuleBase {
	public override string Label
		=> "Gearset";

	protected override string NotReversedLabel
		=> "Gearset";

	protected override string ReversedLabel
		=> "Not";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> right->IsGearsetItem.CompareTo(left->IsGearsetItem);
}