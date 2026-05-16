using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class UntradableFilter : FilteringRuleBase {
	public override string Label
		=> "Untradable";

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->IsUntradable;
}