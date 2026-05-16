using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class ItemLevelFilter : FilteringRuleBase {
	public override string Label
		=> "Item Level";

	public uint MaxLevel;
	public uint MinLevel;

	public override bool HasConfiguration
		=> true;

	public override void DrawConfiguration() {
		ImGui.TextWrapped("Level values are inclusive\n" +
		                  "Ex. 50, 50, will match lvl 50 items only");

		ImGuiHelpers.ScaledDummy(10.0f);

		ImGui.Text("Min ILvl");

		ImGui.SameLine(ImGui.GetContentRegionAvail().X * 1.0f / 3.0f);
		ImGui.InputUInt("##MinLevel", ref MinLevel);

		if (ImGui.IsItemDeactivatedAfterEdit()) {
			System.SystemConfiguration.Save();
		}

		ImGui.Spacing();

		ImGui.Text("Max ILvl");

		ImGui.SameLine(ImGui.GetContentRegionAvail().X * 1.0f / 3.0f);
		ImGui.InputUInt("##MaxLevel", ref MaxLevel);

		if (ImGui.IsItemDeactivatedAfterEdit()) {
			System.SystemConfiguration.Save();
		}
	}

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->ItemLevel >= MinLevel && item->ItemLevel <= MaxLevel;
}