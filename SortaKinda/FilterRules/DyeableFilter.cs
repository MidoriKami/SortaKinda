using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class DyeableFilter : FilteringRuleBase {
	public override string Label
		=> "Dyeable";

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->IsDyeable;
}