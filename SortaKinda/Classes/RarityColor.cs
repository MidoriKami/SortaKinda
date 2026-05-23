using System.Buffers.Binary;
using System.Numerics;
using Lumina.Excel.Sheets;

namespace SortaKinda.Classes;

/// <summary>
/// Helper class for geting the vector color representing item rarity.
/// </summary>
public static class RarityColor {
	public static Vector4 GetRarityColor(uint rarity) {
		var rarityUiColor = GetItemRarityColorType(rarity);

		var rawColor = Services.DataManager.GetExcelSheet<UIColor>().TryGetRow(rarityUiColor, out var color)
			               ? BinaryPrimitives.ReverseEndianness(color.Dark) | 0xFF000000 : 0xFFFFFFFF;

		return UintToVector4(rawColor);
	}

	private static Vector4 UintToVector4(uint color)
		=> new((color & 0xFF) / 255.0f,
			(color >> 8 & 0xFF) / 255.0f,
			(color >> 16 & 0xFF) / 255.0f,
			(color >> 24 & 0xFF) / 255.0f
		);

	private static uint GetItemRarityColorType(uint rarity, bool isEdgeColor = false)
		=> (isEdgeColor ? 548u : 547u) + rarity * 2u;
}