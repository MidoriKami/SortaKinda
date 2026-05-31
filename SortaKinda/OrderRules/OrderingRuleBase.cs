using System;
using System.Text.Json.Serialization;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Classes;

namespace SortaKinda.OrderRules;

public abstract unsafe partial class OrderingRuleBase {
	public abstract string Label { get; }

	public bool IsReversed { get; set; }

	protected abstract string NotReversedLabel { get; }

	protected abstract string ReversedLabel { get; }

	[JsonIgnore]
	public string ButtonLabel
		=> IsReversed ? $"{ReversedLabel} → {NotReversedLabel}" : $"{NotReversedLabel} → {ReversedLabel}";

	public virtual bool IsValid
		=> true;

	public abstract int Compare(InventoryItem* left, InventoryItem* right);

	public int Compare(ItemSlotInfo left, ItemSlotInfo right) {
		var compareResult = Compare(left.Item.Value, right.Item.Value);

		if (compareResult is 0) {
			compareResult = string.Compare(left.Item.Value->Name, right.Item.Value->Name, StringComparison.OrdinalIgnoreCase);
		}

		return compareResult * (IsReversed ? -1 : 1);
	}
}