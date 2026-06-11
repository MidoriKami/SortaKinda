using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class HighQualityFilter : FilteringRuleBase {
	public override string Label
		=> "High Quality";

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->IsHighQuality();
}
