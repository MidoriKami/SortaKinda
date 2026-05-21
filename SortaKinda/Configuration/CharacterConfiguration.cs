using System.Collections.Generic;
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

		Services.PluginLog.Debug("Saving Character.config.json");
		Config.SaveCharacterConfig(this, "Character.config.json");
	}
}