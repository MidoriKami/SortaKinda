using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Dalamud.Excel.Sheets;

namespace SortaKinda.FilterRules;

public class ClassJobFilter : FilteringRuleBase {
	public override string Label
		=> "ClassJob";

	public List<uint> ClassJobIds = [];

	public override bool HasConfiguration => true;

	private string searchString = string.Empty;
	private Regex searchRegex = new(string.Empty);

	public override void DrawConfiguration() {
		DrawSearchBar();
		DrawSearchResults();
	}

	private void DrawSearchBar() {
		var childSize = new Vector2(ImGui.GetContentRegionAvail().X, 26.0f * ImGuiHelpers.GlobalScale);
		using var child = ImRaii.Child("SearchBar", childSize);
		if (!child) return;

		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
		if (ImGui.InputTextWithHint("##SearchBar", "Search . . . ", ref searchString, flags: ImGuiInputTextFlags.AutoSelectAll)) {
			try {
				if (searchString is "") {
					searchRegex = new Regex(string.Empty, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
				}
				else {
					searchRegex = new Regex(searchString, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
				}
			}
			catch (ArgumentException) {
				searchRegex = new Regex(string.Empty, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			}
		}
	}

	private void DrawSearchResults() {
		using var resultChild = ImRaii.Child("Results", ImGui.GetContentRegionAvail());
		if (!resultChild) return;

		foreach (var result in IDataManager.Get().GetExcelSheet<ClassJob>().Where(job => !job.Name.IsEmpty)) {
			var evaluatedName = ISeStringEvaluator.Get().EvaluateFromAddon(698, [result.RowId]).ToString();

			if (!searchRegex.IsMatch(evaluatedName)) continue;

			var cursorPosition = ImGui.GetCursorPos();
			var selectableSize = new Vector2(ImGui.GetContentRegionAvail().X, 24.0f * ImGuiHelpers.GlobalScale);
			if (ImGui.Selectable($"##{evaluatedName}", ClassJobIds.Contains(result.RowId), size: selectableSize)) {
				if (!ClassJobIds.Remove(result.RowId)) {
					ClassJobIds.Add(result.RowId);
				}
			}

			ImGui.SetCursorPos(cursorPosition);
			ImGui.Image(ITextureProvider.Get().GetFromGameIcon(62100 + result.RowId).GetWrapOrEmpty().Handle, ImGuiHelpers.ScaledVector2(24.0f, 24.0f));

			ImGui.SameLine();
			ImGui.AlignTextToFramePadding();
			ImGui.Text(evaluatedName);
		}
	}

	protected override unsafe bool EvaluateItem(InventoryItem* item)
		=> ClassJobIds.Any(job => item->IncludesJob(job));
}