using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class Anything : FilteringRuleBase {
	public override bool IsValid
		=> false;

	public override string Label
		=> "Anything";

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> true;
}