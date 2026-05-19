using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class UniqueFilter : FilteringRuleBase {
	public override string Label
		=> "Unique";

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->IsUnique;
}