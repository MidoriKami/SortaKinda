using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public abstract unsafe class OrderingRuleBase {
	public abstract string Label { get; }

	public abstract int Compare(InventoryItem* left, InventoryItem* right);
}