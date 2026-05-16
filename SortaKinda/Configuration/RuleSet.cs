using System;
using System.Collections.Generic;
using SortaKinda.FilterRules;
using SortaKinda.OrderRules;

namespace SortaKinda.Configuration;

/// <summary>
/// A collection of Filter Rules and Ordering Rules.
/// </summary>
public class RuleSet {
	public string Name = "Name Not Set";
	public Guid RuleSetId = Guid.NewGuid();
	public bool ReverseFill = false;
	public List<FilteringRuleBase> FilterRules = [];
	public List<OrderingRuleBase> OrderingRules = [];
}