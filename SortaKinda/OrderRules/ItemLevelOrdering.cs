using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class ItemLevelOrdering : OrderingRuleBase {
	public override string Label
		=> "Item Level";

	public override string ButtonLabel
		=> IsReversed ? "High → Low" : "Low → High";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> left->ItemLevel.CompareTo(right->ItemLevel);
}