using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using SortaKinda.Abstracts;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.System.Modules;

public class MainInventoryModule : InventoryModuleBase
{
    public override ModuleName ModuleName { get; protected set; } = ModuleName.MainInventory;
    public override IModuleConfig ModuleConfig { get; set; } = new MainModuleConfig();

    private InventoryGrid inventory1 = null!;
    private InventoryGrid inventory2 = null!;
    private InventoryGrid inventory3 = null!;
    private InventoryGrid inventory4 = null!;

    private long mainInventoryLastCount;
    
    protected override void LoadModule()
    {
        ModuleConfig.Configurations ??= new Dictionary<InventoryType, InventoryConfig>
        {
            { InventoryType.Inventory1, new InventoryConfig(InventoryType.Inventory1) },
            { InventoryType.Inventory2, new InventoryConfig(InventoryType.Inventory2) },
            { InventoryType.Inventory3, new InventoryConfig(InventoryType.Inventory3) },
            { InventoryType.Inventory4, new InventoryConfig(InventoryType.Inventory4) },
        };

        inventory1 = new InventoryGrid(InventoryType.Inventory1, this);
        inventory2 = new InventoryGrid(InventoryType.Inventory2, this);
        inventory3 = new InventoryGrid(InventoryType.Inventory3, this);
        inventory4 = new InventoryGrid(InventoryType.Inventory4, this);
    }
    
    public override void SortAll() => SortController.SortInventory(InventoryType.Inventory1, inventory1, inventory2, inventory3, inventory4);

    public override void Update()
    {
        var currentInventoryCount = InventoryController.GetInventoryItemCount(InventoryType.Inventory1, InventoryType.Inventory2, InventoryType.Inventory3, InventoryType.Inventory4);
        
        if (mainInventoryLastCount != currentInventoryCount)
        {
            if(SortaKindaSystem.SystemConfig.SortOnInventoryChange) SortAll();
            mainInventoryLastCount = currentInventoryCount;
        }
    }

    public override void DrawInventoryGrid()
    {
        var region = ImGui.GetContentRegionAvail();

        var firstPosition = new Vector2(0.0f, 0.0f);
        var secondPosition = new Vector2(region.X / 2.0f, 0.0f);
        var thirdPosition = new Vector2(0.0f, region.Y / 2.0f);
        var fourthPosition = new Vector2(region.X / 2.0f, region.Y / 2.0f);
        
        inventory1.Draw(firstPosition);
        inventory2.Draw(secondPosition);
        inventory3.Draw(thirdPosition);
        inventory4.Draw(fourthPosition);
    }
}