using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.Configuration;

/// <summary>
/// Defines a set of slots, and the associated sorting rules to be used.
/// </summary>
public class SlotSet {
	public InventoryType InventoryType;
	public List<int> SlotIndexes = [];
	public Guid RuleSetId;
	public int Priority = 0;

	[JsonIgnore]
	public RuleSet RuleSet
		=> System.SystemConfiguration.RuleSets
			.FirstOrDefault(set => set.RuleSetId == RuleSetId)
				?? throw new Exception($"Rule set {RuleSetId} not found");
}