using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class RepairableFilter : FilteringRuleBase {
	public override string Label
		=> "Repairable";

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->IsRepairable;
}