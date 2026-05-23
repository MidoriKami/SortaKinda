using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.Interop;
using SortaKinda.Classes;
using SortaKinda.FilterRules;
using SortaKinda.OrderRules;

namespace SortaKinda.Configuration;

/// <summary>
/// A collection of Filter Rules and Ordering Rules.
/// </summary>
public unsafe class RuleSet {
	public string Name = "Name Not Set";
	public Guid RuleSetId = Guid.NewGuid();
	public bool ReverseFill = false;
	public List<FilteringRuleBase> FilterRules = [];
	public List<OrderingRuleBase> OrderingRules = [];
	public bool RequireAll = true;
	public Vector4 Color = Vector4.One;

	[JsonIgnore]
	public bool IsValid
		=> FilterRules.Count is not 0;

	public bool IsItemAllowed(InventoryItem* item) {
		if (RequireAll) {
			return FilterRules.All(filter => filter.IsItemAllowed(item));
		}
		else {
			return FilterRules.Any(filter => filter.IsItemAllowed(item));
		}
	}

	public int Comparison(ItemSlotInfo left, ItemSlotInfo right)
		=> Comparison(left.Item, right.Item);

	private int Comparison(Pointer<InventoryItem> left, Pointer<InventoryItem> right) {
		foreach (var orderingRule in OrderingRules) {
			var result = orderingRule.Compare(left, right) * (orderingRule.IsReversed ? -1 : 1);
			if (result is not 0) {
				return result;
			}
		}

		return string.Compare(left.Value->Name, right.Value->Name, StringComparison.OrdinalIgnoreCase);
	}
}