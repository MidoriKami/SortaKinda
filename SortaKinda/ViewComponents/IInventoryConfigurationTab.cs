using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using KamiLib.Classes;

namespace SortaKinda.ViewComponents;

public interface IInventoryConfigurationTab : ITabItem {
    void ITabItem.Draw() {
        using var table = ImRaii.Table("##SortaKindaInventoryConfigTable", 2, ImGuiTableFlags.SizingStretchSame);
        if (!table) return;

        ImGui.TableNextColumn();
        DrawConfigChild();
        
        ImGui.TableNextColumn();
        DrawInventoryChild();
    }

    private static void DrawConfigChild() {
        using var configChild = ImRaii.Child("##ConfigChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding);
        if (!configChild) return;

        System.SortController.Draw();
    }
    
    private void DrawInventoryChild() {
        using var inventoryChild = ImRaii.Child("##InventoryChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding, false, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollbar);
        if (!inventoryChild) return;

        DrawInventory();
    }
    
    void DrawInventory();
}