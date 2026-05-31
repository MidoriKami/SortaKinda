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
	public static readonly Guid IgnoreSlotsId
		= new("4b1a4574-e3fb-51d0-b530-589cb3235ffb");

	[JsonIgnore]
	public RuleSet RuleSet
		=> System.SystemConfiguration.RuleSets
			.FirstOrDefault(set => set.RuleSetId == RuleSetId)
				?? throw new Exception($"Rule set {RuleSetId} not found");
}