using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class WeaponFilter : FilteringRuleBase {
	public override string Label
		=> "Weapon";

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->IsWeapon;
}