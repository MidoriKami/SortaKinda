using System.Text.Json.Serialization;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.OrderRules;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(AlphabeticalOrdering), "AlphabeticalOrdering")]
[JsonDerivedType(typeof(ItemIdOrdering), "ItemIdOrdering")]
[JsonDerivedType(typeof(ItemLevelOrdering), "ItemLevelOrdering")]
[JsonDerivedType(typeof(RarityOrdering), "RarityOrdering")]
[JsonDerivedType(typeof(SellPriceOrdering), "SellPriceOrdering")]
public abstract unsafe class OrderingRuleBase {
	public abstract string Label { get; }

	public bool IsReversed { get; set; }

	public abstract string ButtonLabel { get; }

	public abstract int Compare(InventoryItem* left, InventoryItem* right);
}