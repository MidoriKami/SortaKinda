using System;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Extensions;

namespace SortaKinda.OrderRules;

public class AlphabeticalOrderingRule : OrderingRuleBase {
	public override string Label => "Alphabetical";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> string.Compare(left->Name, right->Name, StringComparison.OrdinalIgnoreCase);
}