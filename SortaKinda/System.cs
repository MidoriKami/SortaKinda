using System.Collections.Generic;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Configuration;
using SortaKinda.FilterRules;
using SortaKinda.OrderRules;
using SortaKinda.Windows;

namespace SortaKinda;

public class System {
	internal static SystemConfiguration SystemConfiguration = null!;
	internal static CharacterConfiguration? CharacterConfiguration;
	internal static WindowSystem WindowSystem = null!;
	internal static ConfigWindow ConfigWindow = null!;

	internal static List<FilteringRuleBase> FilteringRules = [];
	internal static List<OrderingRuleBase> OrderingRules = [];

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