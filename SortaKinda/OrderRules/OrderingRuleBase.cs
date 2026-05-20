using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public abstract unsafe partial class OrderingRuleBase {
	public abstract string Label { get; }

	public bool IsReversed { get; set; }

	protected abstract string NotReversedLabel { get; }

	protected abstract string ReversedLabel { get; }

	public string ButtonLabel
		=> IsReversed ? $"{ReversedLabel} → {NotReversedLabel}" : $"{NotReversedLabel} → {ReversedLabel}";

	public abstract int Compare(InventoryItem* left, InventoryItem* right);
}