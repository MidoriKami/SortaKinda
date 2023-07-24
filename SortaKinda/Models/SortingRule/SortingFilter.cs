using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using SortaKinda.Interfaces;
using SortaKinda.Models.Enum;
using SortaKinda.Views.SortingRuleViews;

namespace SortaKinda.Models;

public class SortingFilter : ISortingFilter
{
    public HashSet<uint> AllowedItemTypes = new();
    public HashSet<string> AllowedNames = new();
    public HashSet<ItemRarity> AllowedRarities = new();

    public bool ItemLevelEnable;
    public int MinItemLevel;
    public int MaxItemLevel = 1000;

    public bool SalePriceEnable;
    public int MinSalePrice;
    public int MaxSalePrice = 999_999_999;

    public bool IsItemSlotAllowed(IInventorySlot slot)
    {
        if (AllowedNames.Count > 0 && !AllowedNames.Any(allowed => Regex.IsMatch(slot.LuminaData?.Name.RawString ?? string.Empty, allowed, RegexOptions.IgnoreCase))) return false;
        if (AllowedItemTypes.Count > 0 && !AllowedItemTypes.Any(allowed => slot.LuminaData?.ItemUICategory.Row == allowed)) return false;

        return true;
    }

    public void DrawConfigTabs()
    {
        if (ImGui.BeginTabItem("Item Type Filter"))
        {
            ItemTypeFilterView.Draw(AllowedItemTypes);
            ImGui.EndTabItem();
        }
        
        if (ImGui.BeginTabItem("Item Name Filter"))
        {
            ItemNameFilterView.Draw(AllowedNames);
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Other Filters"))
        {
            if (ImGui.BeginTable("##OtherFilterTable", 2, ImGuiTableFlags.BordersInnerV, ImGui.GetContentRegionAvail()))
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted("Item Level Range");
                ImGui.Separator();

                ImGui.TableNextColumn();
                ImGui.TextUnformatted("Item Rarities");
                ImGui.Separator();
                
                ImGui.TableNextColumn();
                OtherFilterView.DrawItemLevelFilter(ref ItemLevelEnable, ref MinItemLevel, ref MaxItemLevel);
                ImGuiHelpers.ScaledDummy(5.0f);
                OtherFilterView.DrawSalePriceFilter(ref SalePriceEnable, ref MinSalePrice, ref MaxSalePrice);
                
                ImGui.TableNextColumn();
                OtherFilterView.DrawRarityFilter(AllowedRarities);

                ImGui.EndTable();
            }
            
            ImGui.EndTabItem();
        }
    }
    
    public string GetAllowedItemsString()
    {
        var strings = new List<string>
        {
            AllowedItemTypesString(),
            AllowedItemNameString(),
            AllowedItemLevelString(),
            AllowedSalePriceString(),
            AllowedRaritiesString(),
        };

        strings.RemoveAll(allowedString => allowedString.IsNullOrEmpty());

        return strings.Any() ?  string.Join("\n", strings) : "Any Items";
    }

    private string AllowedItemTypesString()
    {
        if (AllowedItemTypes.Count is 0) return string.Empty;
        
        IEnumerable<string> GetItemTypeStrings()
            => AllowedItemTypes
                .Select(type => LuminaCache<ItemUICategory>.Instance.GetRow(type)?.Name.RawString)
                .OfType<string>();

        var itemTypesString = string.Join(", ", GetItemTypeStrings());

        return "Item Type: " + itemTypesString[..Math.Min(itemTypesString.Length, 55)];
    }

    private string AllowedItemNameString()
    {
        if (AllowedNames.Count is 0) return string.Empty;
        
        var itemNamesString = string.Join(", ", AllowedNames.Select(name => $@"""{name}"""));

        return "Item Name: " + itemNamesString[..Math.Min(itemNamesString.Length, 55)];
    }

    private string AllowedRaritiesString()
    {
        if (AllowedRarities.Count is 0) return string.Empty;
        
        var itemRaritiesString = string.Join(", ", AllowedRarities.Select(rarity => rarity.GetLabel()));

        return "Rarity: " + itemRaritiesString[..Math.Min(itemRaritiesString.Length, 55)];
    }

    private string AllowedItemLevelString()
    {
        if (!ItemLevelEnable) return string.Empty;
        
        return $"Item Level: Min {MinItemLevel}, Max {MaxItemLevel}";
    }
    
    private string AllowedSalePriceString()
    {
        if (!SalePriceEnable) return string.Empty;
        
        return $"Sale Price: Min {MinSalePrice}, Max {MaxSalePrice}";
    }
}