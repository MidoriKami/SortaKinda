using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace SortaKinda.FilterRules;

public class ItemUiCategoryFilter : FilteringRuleBase {
	public override string Label
		=> "Item UI Category";

	public List<uint> ItemUiCategories = [];

	public override bool HasConfiguration => true;

	private string searchString = string.Empty;
	private Regex searchRegex = new(string.Empty);

	public override void DrawConfiguration() {
		DrawSearchBar();
		DrawSearchResults();
	}

	private void DrawSearchBar() {
		var childSize = new Vector2(ImGui.GetContentRegionAvail().X, 26.0f);
		using var child = ImRaii.Child("SearchBar", childSize);
		if (!child) return;

		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.InputTextWithHint("##SearchBar", "Search . . . ", ref searchString)) {
			searchRegex = new Regex(searchString, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		}
	}

	private void DrawSearchResults() {
		using var resultChild = ImRaii.Child("Results", ImGui.GetContentRegionAvail());
		if (!resultChild) return;

		foreach (var result in Services.DataManager.GetExcelSheet<ItemUICategory>()) {
			if (result.Icon is 0) continue;
			if (!searchRegex.IsMatch(result.Name.ToString())) continue;

			var cursorPosition = ImGui.GetCursorPos();
			var selectableSize = new Vector2(ImGui.GetContentRegionAvail().X, 24.0f * ImGuiHelpers.GlobalScale);
			if (ImGui.Selectable($"##{result.Name}", ItemUiCategories.Contains(result.RowId), size: selectableSize)) {
				if (!ItemUiCategories.Remove(result.RowId)) {
					ItemUiCategories.Add(result.RowId);
				}
			}

			ImGui.SetCursorPos(cursorPosition);
			ImGui.Image(Services.TextureProvider.GetFromGameIcon(result.Icon).GetWrapOrEmpty().Handle, ImGuiHelpers.ScaledVector2(24.0f, 24.0f));

			ImGui.SameLine();
			ImGui.AlignTextToFramePadding();
			ImGui.Text(result.Name.ToString());
		}
	}

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> ItemUiCategories.Contains(item->UiCategory.RowId);
}