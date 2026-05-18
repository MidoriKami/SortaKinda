using System;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaKinda.Extensions;

public static unsafe class InventoryContainerExtensions {
	extension(ref InventoryContainer container) {
		public Span<InventoryItem> ItemSpan
			=> new(container.Items, container.Size);
	}
}