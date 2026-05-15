using System;
using System.Collections.Generic;
using System.Numerics;

namespace SortaKinda.Configuration;

/// <summary>
/// Defines a set of slots, and the associated sorting rules to be used.
/// </summary>
public class SlotSet {
	public List<int> SlotIndexes = [];
	public Guid? RuleSetId;
	public Vector4 SetColor = Vector4.One;
	public int Priority = 0;
	public string Name = "Name Not Set";
}