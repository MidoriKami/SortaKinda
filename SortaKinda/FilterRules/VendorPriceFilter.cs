using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.FilterRules;

public class VendorPriceFilter : FilteringRuleBase {
	public override string Label
		=> "Vendor Price";

	public uint MaxPrice;
	public uint MinPrice;

	public override bool HasConfiguration
		=> true;

	public override void DrawConfiguration() {
		ImGui.TextWrapped("Price values are inclusive\n" +
		                  "Ex. 5000, 5000, will match items prices exactly 5000 gil");

		ImGuiHelpers.ScaledDummy(10.0f);

		ImGui.Text("Min Price");

		ImGui.SameLine(ImGui.GetContentRegionAvail().X * 1.0f / 3.0f);
		ImGui.InputUInt("##MinLevel", ref MinPrice);

		if (ImGui.IsItemDeactivatedAfterEdit()) {
			System.SystemConfiguration.Save();
		}

		ImGui.Spacing();

		ImGui.Text("Max Price");

		ImGui.SameLine(ImGui.GetContentRegionAvail().X * 1.0f / 3.0f);
		ImGui.InputUInt("##MaxLevel", ref MaxPrice);

		if (ImGui.IsItemDeactivatedAfterEdit()) {
			System.SystemConfiguration.Save();
		}
	}

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> item->SellPrice <= MaxPrice && item->SellPrice >= MinPrice;
}