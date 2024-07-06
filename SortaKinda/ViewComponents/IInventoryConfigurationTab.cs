using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Classes;
using SortaKinda.Controllers;

namespace SortaKinda.ViewComponents;

public interface IInventoryConfigurationTab : ITabItem {
    void ITabItem.Draw() {
        using var table = ImRaii.Table("##SortaKindaInventoryConfigTable", 2, ImGuiTableFlags.SizingStretchSame);
        if (!table) return;

        ImGui.TableNextColumn();
        using (var configChild = ImRaii.Child("##ConfigChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding)) {
            if (configChild) SortaKindaController.SortController.Draw();
        }
        
        ImGui.TableNextColumn();
        using (var inventoryChild = ImRaii.Child("##InventoryChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding, false, ImGuiWindowFlags.NoMove)) {
            if (inventoryChild) DrawInventory();
        }
    }

    void DrawInventory();
}