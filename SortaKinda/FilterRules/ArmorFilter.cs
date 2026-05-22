using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class ArmorFilter : FilteringRuleBase {
	public override string Label
		=> "Armor";

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->IsArmor;
}