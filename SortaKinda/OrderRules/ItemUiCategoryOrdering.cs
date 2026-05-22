using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class ItemUiCategoryOrdering : OrderingRuleBase {
	public override string Label
		=> "Item Ui Category";

	protected override string NotReversedLabel
		=> "Low";

	protected override string ReversedLabel
		=> "High";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> left->UiCategory.RowId.CompareTo(right->UiCategory.RowId);
}