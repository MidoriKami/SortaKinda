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
	public Guid? RuleSetId;
	public Vector4 SetColor = Vector4.One;
	public int Priority = 0;
	public string Name = "Name Not Set";

	public RuleSet? GetRuleSet()
		=> System.SystemConfiguration.RuleSets.FirstOrDefault(set => set.RuleSetId == RuleSetId);
}