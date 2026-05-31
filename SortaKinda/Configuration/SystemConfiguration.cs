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
	public bool SortOnConfigChange = true;

	public bool EnableSortLogging = false;

	public List<RuleSet> RuleSets = [];
	public InventoryType LastSelectedInventory = InventoryType.Inventory1;

	public static SystemConfiguration Load() {
		var loadedConfig = Config.LoadConfig<SystemConfiguration>("System.config.json");

		// Remove any invalid filter or orderings.
		foreach (var ruleSet in loadedConfig.RuleSets) {
			ruleSet.FilterRules
				.RemoveAll(filter => !filter.IsValid);

			ruleSet.OrderingRules
			    .RemoveAll(filter => !filter.IsValid);
		}

		return loadedConfig;
	}

	public void Save(bool triggerSort = true) {
		if (SortOnConfigChange && triggerSort) {
			System.SortingController.LaunchSortTask();
		}

		Services.PluginLog.Debug("Saving System.config.json");
		Config.SaveConfig(this, "System.config.json");
	}
}