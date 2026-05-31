using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using SortaKinda.Configuration;

namespace SortaKinda.Windows.UiParts;

public static class GeneralConfiguration {
	public static void Draw() {
		var config = System.SystemConfiguration;
		using var spacing = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(6.0f, 6.0f));

		ImGui.Text("Sorting Triggers");
		ImGui.Separator();

		var configChanged = ImGui.Checkbox("Sort on Item Added", ref config.SortOnItemAdded);
		configChanged |= ImGui.Checkbox("Sort on Item Removed", ref config.SortOnItemRemoved);
		configChanged |= ImGui.Checkbox("Sort on Item Changed", ref config.SortOnItemChanged);
		configChanged |= ImGui.Checkbox("Sort on Item Moved", ref config.SortOnItemMoved);
		configChanged |= ImGui.Checkbox("Sort on Item Merged", ref config.SortOnItemMerged);
		configChanged |= ImGui.Checkbox("Sort on Item Split", ref config.SortOnItemSplit);
		configChanged |= ImGui.Checkbox("Sort on Zone Changed", ref config.SortOnZoneChange);
		configChanged |= ImGui.Checkbox("Sort on Job Changed", ref config.SortOnJobChange);
		configChanged |= ImGui.Checkbox("Sort on Login", ref config.SortOnLogin);
		configChanged |= ImGui.Checkbox("Sort on Config Changed", ref config.SortOnConfigChange);

		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.Text("Advanced");
		ImGui.Separator();

		configChanged |= ImGui.Checkbox("Enable Advanced Logging", ref config.EnableSortLogging);

		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.Text("Unassigned Slot Ordering");
		ImGui.Separator();

		configChanged |= ImGui.Checkbox("Enable", ref config.EnableUnassignedOrdering);
		configChanged |= DrawOrderingCombo(config);
		configChanged |= DrawOrderingReverseButton(config);

		if (configChanged) {
			config.Save();
		}
	}

	private static bool DrawOrderingCombo(SystemConfiguration config) {
		ImGui.SameLine();
		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 20.0f * ImGuiHelpers.GlobalScale);

		ImGui.SetNextItemWidth(200.0f * ImGuiHelpers.GlobalScale);
		using var combo = ImRaii.Combo("##Ordering", config.UnassignedSlotOrdering.Label, ImGuiComboFlags.HeightLarge);
		if (!combo) return false;

		foreach (var option in System.GetOrderingRules()) {
			if (ImGui.Selectable(option.Label)) {
				config.UnassignedSlotOrdering = option;
				return true;
			}
		}

		return false;
	}

	private static bool DrawOrderingReverseButton(SystemConfiguration config) {
		var ordering = config.UnassignedSlotOrdering;

		ImGui.SameLine();
		ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 20.0f * ImGuiHelpers.GlobalScale);

		if (ImGui.Button(ordering.ButtonLabel, new Vector2(200.0f * ImGuiHelpers.GlobalScale, 22.0f * ImGuiHelpers.GlobalScale))) {
			ordering.IsReversed = !ordering.IsReversed;
			return true;
		}

		return false;
	}
}