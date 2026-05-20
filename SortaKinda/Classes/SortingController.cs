using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.Inventory;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using SortaKinda.Configuration;

namespace SortaKinda.Classes;

public unsafe class SortingController :  IDisposable {

	private Task? sortingTask;

	public SortingController() {
		Services.GameInventory.InventoryChanged += InventoryChanged;
		Services.ClientState.ClassJobChanged += JobChanged;
		Services.ClientState.TerritoryChanged += TerritoryChanged;
	}

	public void Dispose() {
		Services.GameInventory.InventoryChanged -= InventoryChanged;
		Services.ClientState.ClassJobChanged -= JobChanged;
		Services.ClientState.TerritoryChanged -= TerritoryChanged;
	}

	public void OnLogin() {
		if (System.SystemConfiguration.SortOnLogin) {
			LaunchSortTask();
		}
	}

	private void JobChanged(uint classJobId) {
		if (System.SystemConfiguration.SortOnJobChange) {
			LaunchSortTask();
		}
	}

	private void TerritoryChanged(uint obj) {
		if (System.SystemConfiguration.SortOnZoneChange) {
			LaunchSortTask();
		}
	}

	private void InventoryChanged(IReadOnlyCollection<InventoryEventArgs> events) {
		var config = System.SystemConfiguration;
		var shouldTriggerSort = false;

		foreach(var inventoryEvent in events) {
			shouldTriggerSort |= inventoryEvent.Type switch {
				GameInventoryEvent.Added when config.SortOnItemAdded => true,
				GameInventoryEvent.Removed when config.SortOnItemRemoved => true,
				GameInventoryEvent.Changed when config.SortOnItemChanged => true,
				GameInventoryEvent.Moved when config.SortOnItemMoved => true,
				GameInventoryEvent.Split when config.SortOnItemSplit => true,
				GameInventoryEvent.Merged when config.SortOnItemMerged => true,
				_ => false,
			};
		}

		if (shouldTriggerSort) {
			LaunchSortTask();
		}
	}

	public void LaunchSortTask() {
		if (sortingTask is null || sortingTask.IsCompleted) {
			sortingTask = Task.Run(PerformSort);
		}
		else {
			Services.PluginLog.Warning("Sorting Task Not Started, Already Running.");
		}
	}

	private void PerformSort() {
		var stopwatch = Stopwatch.StartNew();

		try {
			if (System.CharacterConfiguration is not { } characterConfig) return;

			// Get a list of inventories that we actually care about, and resolve to the real inventory type.
			var groupedInventories = characterConfig.Inventories
				.Where(config => config.Value.SlotSets.Count is not 0)
				.GroupBy(config => config.Key.AdjustedInventoryType);

			// Each *real* inventory is independent of each other, so they can be ran in parallel.
			Parallel.ForEach(groupedInventories, inventoryGroup => {
				var adjustedInventoryType = inventoryGroup.Key.AdjustedInventoryType;

				// We're multithreaded, don't just yolo to log. Built logstring, and then report it.
				var logString = new StringBuilder();
				logString.AppendLine();
				logString.AppendLine($"Inventory Group: {inventoryGroup.Key.AdjustedInventoryType}");

				ParseSlotMappings(adjustedInventoryType, out var emptyItemSlots, out var validItemSlots);

				//
				//		Process Slot Sets
				//

				var slotSets = characterConfig.Inventories
					.Where(config => config.Value.SlotSets.Count is not 0)
					.Where(config => config.Key.AdjustedInventoryType == adjustedInventoryType)
					.SelectMany(config => config.Value.SlotSets)
					.OrderByDescending(slotSet => slotSet.Priority);

				foreach (var slotSet in slotSets) {
					if (slotSet.GetRuleSet() is not { } ruleSet) continue;

					var inventoryType = slotSet.InventoryType;

					logString.AppendLine($"\tInventory Config: {inventoryType}");
					logString.AppendLine($"\t\tSlot Set: {slotSet.Name}");
					logString.AppendLine($"\t\tRule Set: {ruleSet.Name}");

					var takenItem = GetInventoryWants(validItemSlots, ruleSet, logString, slotSet);

					logString.AppendLine();

					//
					//		Remove items that we want from the available pool
					//

					foreach (var itemSlotInto in takenItem) {
						validItemSlots.Remove(itemSlotInto);
						logString.AppendLine($"\t\t\tTaking: [{itemSlotInto.VisibleSlotIndex}] {itemSlotInto.Item.Value->Name}");
					}

					logString.AppendLine();

					//
					//		Swap items for each slot
					//

					var slots = ruleSet.ReverseFill ? slotSet.SlotIndexes.AsEnumerable().Reverse() : slotSet.SlotIndexes;

					foreach (var slot in slots) {
						logString.AppendLine($"\t\t\tEvaluating Slot [{slot}]");

						var itemForSlot = inventoryType.GetItem(slot);

						// Check if we have what we want already
						if (takenItem.Count is not 0) {
							var firstTakenItem = takenItem.First();

							if (firstTakenItem.VisibleSlotIndex == slot) {
								logString.AppendLine($"\t\t\t\tSlot already has what we want");

								takenItem.Remove(firstTakenItem);
								logString.AppendLine();
								continue;
							}
						}

						// Is this slot occupied?
						if (itemForSlot->ItemId is not 0) {
							logString.AppendLine($"\t\t\t\tSlot is Occupied with {itemForSlot->Name}");

							// Do we have something we want?
							if (takenItem.Count is not 0) {
								var firstTakenItem = takenItem.First();

								logString.AppendLine($"\t\t\t\tWe want {firstTakenItem.Item.Value->Name}");

								SwapInventorySlots(inventoryType, slot, firstTakenItem);
								logString.AppendLine($"\t\t\t\tSwapping {slot} -> {firstTakenItem.VisibleSlotIndex}");
								takenItem.Remove(firstTakenItem);
							}
							// We don't want this item.
							else {
								logString.AppendLine($"\t\t\t\tWe don't want this item");

								if (emptyItemSlots.Count is not 0) {
									var firstEmptySlot = emptyItemSlots.First();
									SwapInventorySlots(inventoryType, slot, firstEmptySlot);
									logString.AppendLine($"\t\t\t\tSwapping {slot} -> {firstEmptySlot.VisibleSlotIndex}");
									emptyItemSlots.Remove(firstEmptySlot);
								}
								else {
									Services.PluginLog.Warning($"{adjustedInventoryType} is full, unable to move items out of Slot Set {slotSet.Name}");
								}
							}
						}

						// This slot is empty!
						else {
							logString.AppendLine("\t\t\t\tSlot is empty");

							// Do we have something we want?
							if (takenItem.Count is not 0) {
								var firstTakenItem = takenItem.First();
								logString.AppendLine($"\t\t\t\tWe want {firstTakenItem.Item.Value->Name}");
								SwapInventorySlots(inventoryType, slot, firstTakenItem);
								logString.AppendLine($"\t\t\t\tSwapping {slot} -> {firstTakenItem.VisibleSlotIndex}");
								takenItem.Remove(firstTakenItem);
							}
						}

						logString.AppendLine();
					}

					logString.AppendLine();
				}

				if (System.SystemConfiguration.EnableSortLogging) {
					Services.PluginLog.Information(logString.ToString());
				}
			});

			// Trigger AgentInventory Update to update various UI's.
			// Non-networked.
			Services.Framework.RunOnFrameworkThread(() => {
				RaptureAtkModule.Instance()->AgentUpdateFlag |= RaptureAtkModule.AgentUpdateFlags.InventoryUpdate;
			});
		}
		catch (Exception e) {
			Services.PluginLog.Error(e, "Exception in PerformSort");
		}
		finally {
			if (System.SystemConfiguration.EnableSortLogging) {
				Services.PluginLog.Information($"Sorting Completed in {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
			}
		}
	}

	/// <summary>
	/// Perform swapping operation, resolving the inventory types for each item.
	/// </summary>
	/// <param name="inventoryType">AdjustedInventory Type for Rule Set Slot.</param>
	/// <param name="currentSlot">The InventorySorter-based index for the item.</param>
	/// <param name="targetItem">The Target Item Slot Info to swap.</param>
	private static void SwapInventorySlots(InventoryType inventoryType, int currentSlot, ItemSlotInfo targetItem) {
		var itemOrderInfoLeft =  inventoryType.GetItemOrderData(currentSlot);
		var itemOrderInfoRight =  targetItem.InventoryType.GetItemOrderData(targetItem.VisibleSlotIndex);

		(*itemOrderInfoLeft, *itemOrderInfoRight) = (*itemOrderInfoRight, *itemOrderInfoLeft);
	}

	/// <summary>
	/// Parses a real inventory to figure out which slots have real items, and which slots are empty.
	/// </summary>
	/// <param name="adjustedInventoryType">The real inventory type to parse.</param>
	/// <param name="emptyItemSlots">List of Visible Slot Indexes that have no item.</param>
	/// <param name="validItemSlots">List of Visible Slot Indexes with valid items.</param>
	private static void ParseSlotMappings(InventoryType adjustedInventoryType, out List<ItemSlotInfo> emptyItemSlots, out List<ItemSlotInfo> validItemSlots) {
		emptyItemSlots = [];
		validItemSlots = [];

		if (System.CharacterConfiguration is not { } characterConfig) return;

		var inventorySorter = adjustedInventoryType.InventorySorter;

		var ruleSetSlots = characterConfig.Inventories
			.GroupBy(pair => pair.Key.AdjustedInventoryType)
			.ToDictionary(
				pair => pair.Key.AdjustedInventoryType,
				pair => pair.SelectMany(entry => entry.Value.SlotSets.SelectMany(set => set.SlotIndexes)));

		foreach (var (index, _) in inventorySorter->Items.Index()) {
			var item = inventorySorter->GetInventoryItem(index);

			if (item->ItemId is 0) {

				var isSlotInUse = ruleSetSlots.TryGetValue(adjustedInventoryType, out var slotSetIndexes) && slotSetIndexes.Contains(index);

				// Only allow moving items into slots that are not in use.
				if (!isSlotInUse) {
					emptyItemSlots.Add(new ItemSlotInfo(index, item, adjustedInventoryType));
				}
			}
			else {
				validItemSlots.Add(new ItemSlotInfo(index, item, adjustedInventoryType));
			}
		}
	}

	/// <summary>
	/// Gets a list of items that this slotset and ruleset wants.
	/// </summary>
	/// <param name="validItemSlots">Items to take from, this is mutated.</param>
	/// <param name="ruleSet"></param>
	/// <param name="logString"></param>
	/// <param name="slotSet"></param>
	/// <returns></returns>
	private static List<ItemSlotInfo> GetInventoryWants(List<ItemSlotInfo> validItemSlots, RuleSet ruleSet, StringBuilder logString, SlotSet slotSet) {
		List<ItemSlotInfo> wantedItemSlots = [];

		foreach (var itemSlot in validItemSlots.Where(itemSlot => ruleSet.IsItemAllowed(itemSlot.Item))) {
			wantedItemSlots.Add(itemSlot);
			logString.AppendLine($"\t\t\tWanted: [{itemSlot.VisibleSlotIndex}] {itemSlot.Item.Value->Name}");
		}

		// Sort the items we want, so that the ones we take are ordered correctly.
		wantedItemSlots.Sort(ruleSet.Comparison);

		return wantedItemSlots.Take(slotSet.SlotIndexes.Count).ToList();
	}
}