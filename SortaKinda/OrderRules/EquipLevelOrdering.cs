using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

public class EquipLevelOrdering : OrderingRuleBase {
	public override string Label
		=> "Equip Level";

	protected override string NotReversedLabel
		=> "High";

	protected override string ReversedLabel
		=> "Low";

	public override unsafe int Compare(InventoryItem* left, InventoryItem* right)
		=> right->EquipLevel.CompareTo(left->EquipLevel);
}
