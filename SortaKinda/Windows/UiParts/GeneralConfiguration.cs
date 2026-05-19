using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;

namespace SortaKinda.Windows.UiParts;

public static class GeneralConfiguration {
	public static void Draw() {
		var config = System.SystemConfiguration;
		using var spacing = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(4.0f, 6.0f));

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

		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.Text("Advanced");
		ImGui.Separator();

		configChanged |= ImGui.Checkbox("Enable Advanced Logging", ref config.EnableSortLogging);

		if (configChanged) {
			config.Save();
		}
	}
}