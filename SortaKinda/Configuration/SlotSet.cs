using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.Configuration;

/// <summary>
/// Defines a set of slots, and the associated sorting rules to be used.
/// </summary>
public class SlotSet {
	public InventoryType InventoryType;
	public List<int> SlotIndexes = [];
	public required RuleSet RuleSet;
	public int Priority = 0;
}