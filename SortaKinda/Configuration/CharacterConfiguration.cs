using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Utilities;

namespace SortaKinda.Configuration;

/// <summary>
/// Represents all the settings for a specific character.
/// </summary>
public class CharacterConfiguration {
	public Dictionary<InventoryType, InventoryConfig> Inventories = [];

	public static CharacterConfiguration Load()
		=> Config.LoadCharacterConfig<CharacterConfiguration>("Character.config.json");

	public void Save() {
		if (System.SystemConfiguration.SortOnConfigChange) {
			System.SortingController.LaunchSortTask();
		}

		IPluginLog.Get().Debug("Saving Character.config.json");
		Config.SaveCharacterConfig(this, "Character.config.json");
	}

	/// <summary>
	/// Removes any slot sets that are using any rulesets that no longer exist.
	/// </summary>
	/// <returns>True if a resave is required.</returns>
	public bool PurgeInvalidSlotSets() {
		var anyPurged = false;
		foreach (var (_, inventoryConfig) in Inventories) {
			var numRemoved = inventoryConfig.SlotSets
			    .RemoveAll(set => set.RuleSetId == Guid.Empty || System.SystemConfiguration.RuleSets
			        .All(systemSet => systemSet.RuleSetId != set.RuleSetId));

			if (numRemoved > 0) {
				anyPurged = true;
			}
		}

		return anyPurged;
	}
}