using System.Collections.Generic;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Classes;

namespace SortaKinda.FilterRules;

public class RarityFilter : FilteringRuleBase {
	public override string Label
		=> "Rarity";

	public List<uint> Rarities = [];

	public override bool HasConfiguration => true;

	public override void DrawConfiguration() {
		foreach (var rarity in (List<uint>) [1, 2, 3, 4, 7]) {
			if (ImWidget.DrawColoredSelectable(RarityColor.GetRarityColor(rarity), $"Rarity #{rarity}", Rarities.Contains(rarity))) {
				if (!Rarities.Remove(rarity)) {
					Rarities.Add(rarity);
				}
			}

			ImGui.Spacing();
		}
	}

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> Rarities.Contains(item->ItemRarity);
}