using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace SortaKinda.Extensions;

public static unsafe class ItemOrderModuleSorterExtensions {
    extension(ref ItemOrderModuleSorter sorter) {
        public InventoryItem* GetInventoryItem(long slotIndex) {
            if (sorter.Items.LongCount <= slotIndex) return null;

            var item = sorter.Items[slotIndex].Value;
            if (item == null) return null;

            var container = InventoryManager.Instance()->GetInventoryContainer(sorter.InventoryType + item->Page);
            if (container == null) return null;

            return container->GetInventorySlot(item->Slot);
        }
    }
}
