using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.AddonControllers;
using SortaKinda.Classes;
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
	internal static SortingController SortingController = null!;
	internal static SortingButtonController SortingButtonController = null!;

	internal static List<Type> FilteringRuleTypes = [];
	internal static List<Type> OrderingRuleTypes = [];

	internal static List<FilteringRuleBase> GetFilteringRules()
		=> FilteringRuleTypes
		   .Select(type => (FilteringRuleBase?)Activator.CreateInstance(type))
		   .OfType<FilteringRuleBase>()
		   .OrderBy(rule => rule.Label)
		   .ToList();

	internal static List<OrderingRuleBase> GetOrderingRules()
		=> OrderingRuleTypes
		   .Select(type => (OrderingRuleBase?)Activator.CreateInstance(type))
		   .OfType<OrderingRuleBase>()
		   .OrderBy(rule => rule.Label)
		   .ToList();

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
		InventoryType.SaddleBag1,
		InventoryType.SaddleBag2,
		InventoryType.PremiumSaddleBag1,
		InventoryType.PremiumSaddleBag2,
	];
}