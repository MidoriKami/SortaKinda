using System;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class AlphabeticalOrdering : OrderingRuleBase {
	public override string Label
		=> "Alphabetical";

	public override string ButtonLabel
		=> IsReversed ? "Z → A" : "A → Z";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> string.Compare(left->Name, right->Name, StringComparison.OrdinalIgnoreCase);
}