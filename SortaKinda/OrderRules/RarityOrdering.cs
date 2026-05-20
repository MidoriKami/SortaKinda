using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class RarityOrdering : OrderingRuleBase {
	public override string Label
		=> "Rarity";

	protected override string NotReversedLabel
		=> "Common";

	protected override string ReversedLabel
		=> "Relic";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> left->ItemRarity.CompareTo(right->ItemRarity);
}