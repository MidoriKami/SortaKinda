using KamiLib.Configuration;

namespace SortaKinda.Classes;

public class SystemConfig : CharacterConfiguration {
	public bool SortOnItemAdded = true;
	public bool SortOnItemRemoved = true;
	public bool SortOnItemChanged;
	public bool SortOnItemMoved;
	public bool SortOnItemMerged;
	public bool SortOnItemSplit;
	public bool SortOnZoneChange = true;
	public bool SortOnJobChange = true;
	public bool SortOnLogin = true;
	public bool ReorderUnsortedItems;
}