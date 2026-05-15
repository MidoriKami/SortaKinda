using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SortaKinda.FilterRules;

public unsafe class InGearset : FilteringRuleBase {
	public override string Label => "In a Gearset";

	protected override bool EvaluateItem(InventoryItem* item) {
		foreach (var enabledGearsetIndex in RaptureGearsetModule.Instance()->EnabledGearsetIndex2EntryIndex) {
			if (enabledGearsetIndex is 0) continue;

			foreach (ref var itemInGearset in RaptureGearsetModule.Instance()->Entries[enabledGearsetIndex].Items) {
				if (itemInGearset.ItemId == item->GetItemId()) return true;
			}
		}

		return false;
	}
}