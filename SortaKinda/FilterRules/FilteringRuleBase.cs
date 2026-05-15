using System;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public abstract unsafe partial class FilteringRuleBase {
	public Guid RuleId { get; }

	public abstract string Label { get; }

	/// <summary>
	/// Gets a value indicating if this rule is being allowed, or being blocked.
	/// </summary>
	public bool IsAllowed { get; set; } = true;

	public bool HasConfiguration { get; set; } = false;
	public virtual void DrawConfiguration() {
	}

	protected abstract bool EvaluateItem(InventoryItem* item);

	public bool IsItemAllowed(InventoryItem* item)
		=> EvaluateItem(item) && IsAllowed;
}