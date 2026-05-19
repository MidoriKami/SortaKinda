using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class RarityOrdering : OrderingRuleBase {
	public override string Label
		=> "Rarity";

	public override string ButtonLabel
		=> IsReversed ? "Common → Relic" : "Relic → Common";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> left->ItemRarity.CompareTo(right->ItemRarity);
}