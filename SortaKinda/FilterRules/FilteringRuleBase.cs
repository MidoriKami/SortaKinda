using System.Text.Json.Serialization;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(GearsetFilter), "GearsetFilter")]
[JsonDerivedType(typeof(ItemUiCategoryFilter), "ItemUiCategoryFilter")]
[JsonDerivedType(typeof(NameFilter), "NameFilter")]
[JsonDerivedType(typeof(UntradableFilter), "UntradableFilter")]
[JsonDerivedType(typeof(UniqueFilter), "UniqueFilter")]
[JsonDerivedType(typeof(DyeableFilter), "DyeableFilter")]
[JsonDerivedType(typeof(CollectableFilter), "CollectableFilter")]
[JsonDerivedType(typeof(RepairableFilter), "RepairableFilter")]
[JsonDerivedType(typeof(ItemLevelFilter), "ItemLevelFilter")]
[JsonDerivedType(typeof(VendorPriceFilter), "VendorPriceFilter")]
[JsonDerivedType(typeof(EquipLevelFilter), "EquipLevelFilter")]
[JsonDerivedType(typeof(ItemIdFilter), "ItemIdFilter")]
public abstract unsafe class FilteringRuleBase {
	/// <summary>
	/// Gets the label that will be displayed for this filter.
	/// </summary>
	public abstract string Label { get; }

	/// <summary>
	/// Gets a value indicating if this rule is being allowed, or being blocked.
	/// </summary>
	public bool IsAllowed { get; set; } = true;

	/// <summary>
	/// Set to true to enable the "Configure" button.
	/// </summary>
	public virtual bool HasConfiguration => false;

	/// <summary>
	/// Drawn in a popup window to configure this filter.
	/// </summary>
	public virtual void DrawConfiguration() { }

	/// <summary>
	/// Function that evaluates the given item to see if this filter would allow it.
	/// </summary>
	protected abstract bool EvaluateItem(InventoryItem* item);

	/// <summary>
	/// Public function for evaluating if this item should be allowed.
	/// </summary>
	public bool IsItemAllowed(InventoryItem* item)
		=> EvaluateItem(item) && IsAllowed;
}