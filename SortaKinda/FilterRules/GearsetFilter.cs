using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public unsafe class GearsetFilter : FilteringRuleBase {
	public override string Label
		=> "Gearset";

	protected override bool EvaluateItem(InventoryItem* item)
		=> item->IsGearsetItem;
}