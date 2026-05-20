using System;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class AlphabeticalOrdering : OrderingRuleBase {
	public override string Label
		=> "Alphabetical";

 	protected override string NotReversedLabel
		=> "A";

	protected override string ReversedLabel
		=> "Z";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> string.Compare(left->Name, right->Name, StringComparison.OrdinalIgnoreCase);
}