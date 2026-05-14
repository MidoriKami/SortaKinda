using System.Collections.Generic;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Configuration;
using SortaKinda.Windows;

namespace SortaKinda;

public class System {
	public static SystemConfiguration SystemConfiguration = null!;
	public static CharacterConfiguration? CharacterConfiguration;
	public static WindowSystem WindowSystem = null!;
	public static ConfigWindow ConfigWindow = null!;

	public static readonly List<InventoryType> AllowedInventories = [
		InventoryType.Inventory1,
		InventoryType.Inventory2,
		InventoryType.Inventory3,
		InventoryType.Inventory4,
		InventoryType.ArmoryMainHand,
		InventoryType.ArmoryHead,
		InventoryType.ArmoryBody,
		InventoryType.ArmoryHands,
		InventoryType.ArmoryLegs,
		InventoryType.ArmoryFeets,
		InventoryType.ArmoryOffHand,
		InventoryType.ArmoryEar,
		InventoryType.ArmoryNeck,
		InventoryType.ArmoryWrist,
		InventoryType.ArmoryRings,
		InventoryType.ArmorySoulCrystal,
	];
}