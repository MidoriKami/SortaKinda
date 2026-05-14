using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Utilities;

namespace SortaKinda.Configuration;

/// <summary>
/// Primary configuration file that is shared across all characters for controlling general sorting behavior.
/// </summary>
public class SystemConfiguration {
	public int Version = 3;

	public bool SortOnItemAdded = true;
	public bool SortOnItemRemoved = true;
	public bool SortOnItemChanged = false;
	public bool SortOnItemMoved = false;
	public bool SortOnItemMerged = false;
	public bool SortOnItemSplit = false;
	public bool SortOnZoneChange = true;
	public bool SortOnJobChange = true;
	public bool SortOnLogin = true;

	public List<RuleSet> RuleSets = [];
	public InventoryType LastSelectedInventory = InventoryType.Inventory1;

	public static SystemConfiguration Load()
		=> Config.LoadConfig<SystemConfiguration>("System.config.json");

	public void Save()
		=> Config.SaveConfig(this, "System.config.json");
}