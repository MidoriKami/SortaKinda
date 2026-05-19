using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class ItemIdFilter : FilteringRuleBase {
	public override string Label
		=> "Item Id";

	public uint MaxId;
	public uint MinId;

	public override bool HasConfiguration
		=> true;

	public override void DrawConfiguration() {
		ImGui.TextWrapped("Item Id values are inclusive\n" +
		                  "Ex. 5000, 5000, will match the one item with id 5000");

		ImGuiHelpers.ScaledDummy(10.0f);

		ImGui.Text("Min Id");

		ImGui.SameLine(ImGui.GetContentRegionAvail().X * 1.0f / 3.0f);
		ImGui.InputUInt("##MinId", ref MinId);

		if (ImGui.IsItemDeactivatedAfterEdit()) {
			System.SystemConfiguration.Save();
		}

		ImGui.Spacing();

		ImGui.Text("Max Id");

		ImGui.SameLine(ImGui.GetContentRegionAvail().X * 1.0f / 3.0f);
		ImGui.InputUInt("##MaxId", ref MaxId);

		if (ImGui.IsItemDeactivatedAfterEdit()) {
			System.SystemConfiguration.Save();
		}
	}

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->ItemId <= MaxId && item->ItemId >= MinId;
}