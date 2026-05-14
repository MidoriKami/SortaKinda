using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public abstract unsafe class FilteringRuleBase {
	public abstract string Label { get; }

	public abstract bool IsItemAllowed(InventoryItem* item);
}