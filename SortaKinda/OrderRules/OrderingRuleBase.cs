using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public abstract unsafe partial class OrderingRuleBase {
	public abstract string Label { get; }

	public bool IsReversed { get; set; }

	public abstract string ButtonLabel { get; }

	public abstract int Compare(InventoryItem* left, InventoryItem* right);
}