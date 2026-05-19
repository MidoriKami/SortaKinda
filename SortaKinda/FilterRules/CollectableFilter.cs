using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class CollectableFilter : FilteringRuleBase {
	public override string Label
		=> "Collectable";

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> ItemUtil.IsCollectible(item->ItemId);
}