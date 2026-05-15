using System;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public abstract unsafe partial class OrderingRuleBase {
	public Guid RuleId { get; }

	public abstract string Label { get; }

	public abstract int Compare(InventoryItem* left, InventoryItem* right);
}