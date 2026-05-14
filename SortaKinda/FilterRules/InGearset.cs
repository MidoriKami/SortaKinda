using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SortaKinda.FilterRules;

public unsafe class InGearset : FilteringRuleBase {
	public override string Label => "In a Gearset";

	public override bool IsItemAllowed(InventoryItem* item)
		=> AllowGearsetItems && IsItemInGearset(item);

	public bool AllowGearsetItems { get; set; } = true;

	private static bool IsItemInGearset(InventoryItem* item) {
		foreach (var enabledGearsetIndex in RaptureGearsetModule.Instance()->EnabledGearsetIndex2EntryIndex) {
			if (enabledGearsetIndex is 0) continue;

			foreach (ref var itemInGearset in RaptureGearsetModule.Instance()->Entries[enabledGearsetIndex].Items) {
				if (itemInGearset.ItemId == item->GetItemId()) return true;
			}
		}

		return false;
	}
}