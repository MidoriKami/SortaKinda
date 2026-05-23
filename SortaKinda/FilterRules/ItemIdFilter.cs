using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class ItemIdFilter : FilteringRuleBase {
	public override string Label
		=> "Item Id";

	public uint ItemId;

	public override bool HasConfiguration
		=> true;

	public override void DrawConfiguration() {
		ImGuiHelpers.ScaledDummy(10.0f);

		ImGui.Text("Item Id");

		ImGui.SameLine(ImGui.GetContentRegionAvail().X * 1.0f / 3.0f);
		ImGui.InputUInt("##ItemId", ref ItemId);

		if (ImGui.IsItemDeactivatedAfterEdit()) {
			System.SystemConfiguration.Save();
		}
	}

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->ItemId == ItemId;
}