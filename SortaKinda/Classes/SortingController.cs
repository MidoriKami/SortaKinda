using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Game.Inventory;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using SortaKinda.Configuration;
using AgentId = Dalamud.Game.Agent.AgentId;

namespace SortaKinda.Classes;

public unsafe class SortingController :  IDisposable {

	private Task? sortingTask;

	public SortingController() {
		Services.GameInventory.InventoryChanged += InventoryChanged;
		Services.ClientState.ClassJobChanged += JobChanged;
		Services.ClientState.TerritoryChanged += TerritoryChanged;
		Services.AgentLifecycle.RegisterListener(AgentEvent.PreReceiveEvent, AgentId.InventoryContext, OnContextMenuEvent);
	}

	public void Dispose() {
		Services.GameInventory.InventoryChanged -= InventoryChanged;
		Services.ClientState.ClassJobChanged -= JobChanged;
		Services.ClientState.TerritoryChanged -= TerritoryChanged;
		Services.AgentLifecycle.UnregisterListener(OnContextMenuEvent);
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

	private void OnContextMenuEvent(AgentEvent type, AgentArgs args) {
		if (System.CharacterConfiguration is not { } characterConfiguration) return;
		if (args is not AgentReceiveEventArgs receiveEventArgs) return;
		if (receiveEventArgs.EventKind is not (71 or 72)) return;
		if (receiveEventArgs.ValueCount < 2) return;
		if (!receiveEventArgs.AtkValueEnumerable.ElementAt(1).TryGet(out int eventKind)) return;

		//  v17 = Component::GUI::AtkValue_GetInt(&values[1])
		// 	if ( eventKind == 71 || eventKind == 72 )
		// 		v18 = this->EventIds[v17 + 8];
		//	switch ( v18 )
		//		case 40:
		//			Do Inventory Sorting

		var agentContext = AgentInventoryContext.Instance();
		var eventIndex = eventKind + 8;
		var eventId = agentContext->EventIds[eventIndex];

		// Inventory, Retainer, Armoury, Buddy, PremiumBuddy
		if (eventId is not 40) return;

		var targetInventory = agentContext->TargetInventoryId.AdjustedInventoryType;

		var anySlotSetsForInventory = characterConfiguration.Inventories.Values
			.Any(inventory => inventory.SlotSets
				.Any(set => set.InventoryType.AdjustedInventoryType == targetInventory));

		if (!anySlotSetsForInventory) return;

		Services.PluginLog.Information("SortaKinda has intercepted the Sort command from context menu.");
		LaunchSortTask();
		args.PreventOriginal();
		agentContext->Hide();
	}

	public void LaunchSortTask() {
		if (sortingTask is null || sortingTask.IsCompleted) {
			sortingTask = Task.Run(PerformSort);
		}
		else {
			Services.PluginLog.Warning("Sorting Task Not Started, Already Running.");
		}
	}

	private static void PerformSort() {
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
					var ruleSet = slotSet.RuleSet;

					// Skip this slot set, if no filters are defined.
					if (!ruleSet.IsValid) continue;

					var inventoryType = slotSet.InventoryType;

					logString.AppendLine($"\tInventory Config: {inventoryType}");
					logString.AppendLine($"\t\tSlot Set: {slotSet.RuleSet.Name}");

					var takenItems = GetInventoryWants(validItemSlots, ruleSet, logString, slotSet);

					logString.AppendLine();

					//
					//		Remove items that we want from the available pool
					//

					foreach (var itemSlotInto in takenItems) {
						validItemSlots.Remove(itemSlotInto);
						logString.AppendLine($"\t\t\tTaking: [{itemSlotInto.VisibleSlotIndex}] {itemSlotInto.Item.Value->Name}");
					}

					logString.AppendLine();

					//
					//		Swap items for each slot
					//

					var slots = ruleSet.ReverseFill ? slotSet.SlotIndexes.AsEnumerable().Reverse() : slotSet.SlotIndexes;

					foreach (var slot in slots) {
						var itemForSlot = inventoryType.GetItem(slot);
						var adjustedSlotIndex = (int)( slot + inventoryType.InventorySorter->ItemsPerPage * (inventoryType - inventoryType.AdjustedInventoryType) );

						logString.AppendLine($"\t\t\tEvaluating Slot [{adjustedSlotIndex}]");

						// Check if we have what we want already
						if (takenItems.Count is not 0) {
							var firstTakenItem = takenItems.First();

							if (firstTakenItem.VisibleSlotIndex == adjustedSlotIndex) {
								logString.AppendLine("\t\t\t\tSlot already has what we want");

								takenItems.Remove(firstTakenItem);
								logString.AppendLine();
								continue;
							}
						}

						// Is this slot occupied?
						if (itemForSlot->ItemId is not 0) {
							logString.AppendLine($"\t\t\t\tSlot is Occupied with {itemForSlot->Name}");

							// Do we have something we want?
							if (takenItems.Count is not 0) {
								var firstTakenItem = takenItems.First();

								logString.AppendLine($"\t\t\t\tWe want {firstTakenItem.Item.Value->Name} from slot [{firstTakenItem.VisibleSlotIndex}]");

								SwapInventorySlots(inventoryType, slot, firstTakenItem);

								// If there is an item that we want, at all, but it's in this slot, we just moved it. Update its known position.
								var outdatedItemInfo = takenItems.FirstOrDefault(item => item.VisibleSlotIndex == adjustedSlotIndex);
								outdatedItemInfo?.VisibleSlotIndex = firstTakenItem.VisibleSlotIndex;

								logString.AppendLine($"\t\t\t\tSwapping {adjustedSlotIndex} -> {firstTakenItem.VisibleSlotIndex}");
								takenItems.Remove(firstTakenItem);
							}
							// We don't want this item.
							else {
								logString.AppendLine("\t\t\t\tWe don't want this item");

								if (emptyItemSlots.Count is not 0) {
									var firstEmptySlot = emptyItemSlots.First();

									SwapInventorySlots(inventoryType, slot, firstEmptySlot);

									logString.AppendLine($"\t\t\t\tSwapping {adjustedSlotIndex} -> {firstEmptySlot.VisibleSlotIndex}");
									emptyItemSlots.Remove(firstEmptySlot);
								}
								else {
									Services.PluginLog.Warning($"{adjustedInventoryType} is full, unable to move items out of Slot Set {slotSet.RuleSet.Name}");
								}
							}
						}

						// This slot is empty!
						else {
							logString.AppendLine("\t\t\t\tSlot is empty");

							// Do we have something we want?
							if (takenItems.Count is not 0) {
								var firstTakenItem = takenItems.First();
								logString.AppendLine($"\t\t\t\tWe want {firstTakenItem.Item.Value->Name}");

								SwapInventorySlots(inventoryType, slot, firstTakenItem);

								logString.AppendLine($"\t\t\t\tSwapping {adjustedSlotIndex} -> {firstTakenItem.VisibleSlotIndex}");
								takenItems.Remove(firstTakenItem);
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
			var sortingResult = $"Sorting Completed in {stopwatch.Elapsed.TotalMilliseconds:F4} ms";

			if (System.SystemConfiguration.EnableSortLogging) {
				Services.PluginLog.Information(sortingResult);
			}
			else {
				Services.PluginLog.Debug(sortingResult);
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

		var inventorySorter = adjustedInventoryType.InventorySorter;

		foreach (var (index, _) in inventorySorter->Items.Index()) {
			var item = inventorySorter->GetInventoryItem(index);
			if (item is null) continue;

			if (item->ItemId is 0) {

				// Only allow moving items into slots that are not in use.
				if (!IsSlotReserved(index)) {
					emptyItemSlots.Add(new ItemSlotInfo(index, item, adjustedInventoryType));
				}
			}
			else {
				validItemSlots.Add(new ItemSlotInfo(index, item, adjustedInventoryType));
			}
		}
	}

	/// <summary>
	/// Checks each config to see if this real-slot is reserved for any slot set.
	/// </summary>
	/// <param name="inventorySlot">The real-sorter slot index</param>
	/// <returns>True if the slot is wanted by any slotset.</returns>
	private static bool IsSlotReserved(int inventorySlot) {
		if (System.CharacterConfiguration is not { } characterConfig) return false;

		foreach (var (inventoryType, config) in characterConfig.Inventories) {
			foreach (var slotSet in config.SlotSets) {
				foreach (var slot in slotSet.SlotIndexes) {
					var adjustedSlotIndex = (int)( slot + inventoryType.InventorySorter->ItemsPerPage * (inventoryType - inventoryType.AdjustedInventoryType) );

					if (inventorySlot == adjustedSlotIndex) return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Gets a list of items that this slot set and ruleset wants.
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