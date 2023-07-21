using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using ImGuiScene;
using KamiLib.Utilities;
using SortaKinda.Abstracts;
using SortaKinda.Interfaces;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.System.Modules;

public class ArmoryInventoryModule : InventoryModuleBase
{
    private readonly UldWrapper armouryBoard;
    private readonly Dictionary<InventoryType, int> lastItemCounts = new();
    private readonly Dictionary<InventoryType, TextureWrap?> leftTabTextures = new();
    private readonly Dictionary<InventoryType, TextureWrap?> rightTabTextures = new();

    private Dictionary<InventoryType, IInventoryGrid> armoryTabs = null!;

    private InventoryType selectedTab = InventoryType.ArmoryMainHand;

    public ArmoryInventoryModule()
    {
        armouryBoard = Service.PluginInterface.UiBuilder.LoadUld("ui/uld/ArmouryBoard.uld");

        leftTabTextures.Add(InventoryType.ArmoryMainHand, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 0));
        leftTabTextures.Add(InventoryType.ArmoryHead, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 1));
        leftTabTextures.Add(InventoryType.ArmoryBody, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 2));
        leftTabTextures.Add(InventoryType.ArmoryHands, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 3));
        leftTabTextures.Add(InventoryType.ArmoryLegs, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 5));
        leftTabTextures.Add(InventoryType.ArmoryFeets, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 6));

        rightTabTextures.Add(InventoryType.ArmoryOffHand, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 7));
        rightTabTextures.Add(InventoryType.ArmoryEar, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 8));
        rightTabTextures.Add(InventoryType.ArmoryNeck, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 9));
        rightTabTextures.Add(InventoryType.ArmoryWrist, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 10));
        rightTabTextures.Add(InventoryType.ArmoryRings, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 11));
        rightTabTextures.Add(InventoryType.ArmorySoulCrystal, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 12));
    }
    public override ModuleName ModuleName { get; protected set; } = ModuleName.ArmoryInventory;
    public override IModuleConfig ModuleConfig { get; set; } = new GenericModuleConfig();

    public override void Dispose()
    {
        foreach (var loadedTexture in leftTabTextures) loadedTexture.Value?.Dispose();
        foreach (var loadedTexture in rightTabTextures) loadedTexture.Value?.Dispose();

        armouryBoard.Dispose();

        base.Dispose();
    }

    protected override void LoadModule()
    {
        ModuleConfig.Configurations ??= new Dictionary<InventoryType, InventoryConfig>
        {
            { InventoryType.ArmoryMainHand, new InventoryConfig(InventoryType.ArmoryMainHand) },
            { InventoryType.ArmoryOffHand, new InventoryConfig(InventoryType.ArmoryOffHand) },
            { InventoryType.ArmoryHead, new InventoryConfig(InventoryType.ArmoryHead) },
            { InventoryType.ArmoryBody, new InventoryConfig(InventoryType.ArmoryBody) },
            { InventoryType.ArmoryHands, new InventoryConfig(InventoryType.ArmoryHands) },
            { InventoryType.ArmoryLegs, new InventoryConfig(InventoryType.ArmoryLegs) },
            { InventoryType.ArmoryFeets, new InventoryConfig(InventoryType.ArmoryFeets) },
            { InventoryType.ArmoryEar, new InventoryConfig(InventoryType.ArmoryEar) },
            { InventoryType.ArmoryNeck, new InventoryConfig(InventoryType.ArmoryNeck) },
            { InventoryType.ArmoryWrist, new InventoryConfig(InventoryType.ArmoryWrist) },
            { InventoryType.ArmoryRings, new InventoryConfig(InventoryType.ArmoryRings) },
            { InventoryType.ArmorySoulCrystal, new InventoryConfig(InventoryType.ArmorySoulCrystal) }
        };

        armoryTabs = new Dictionary<InventoryType, IInventoryGrid>
        {
            { InventoryType.ArmoryMainHand, new InventoryGrid(InventoryType.ArmoryMainHand, this) },
            { InventoryType.ArmoryOffHand, new InventoryGrid(InventoryType.ArmoryOffHand, this) },
            { InventoryType.ArmoryHead, new InventoryGrid(InventoryType.ArmoryHead, this) },
            { InventoryType.ArmoryBody, new InventoryGrid(InventoryType.ArmoryBody, this) },
            { InventoryType.ArmoryHands, new InventoryGrid(InventoryType.ArmoryHands, this) },
            { InventoryType.ArmoryLegs, new InventoryGrid(InventoryType.ArmoryLegs, this) },
            { InventoryType.ArmoryFeets, new InventoryGrid(InventoryType.ArmoryFeets, this) },
            { InventoryType.ArmoryEar, new InventoryGrid(InventoryType.ArmoryEar, this) },
            { InventoryType.ArmoryNeck, new InventoryGrid(InventoryType.ArmoryNeck, this) },
            { InventoryType.ArmoryWrist, new InventoryGrid(InventoryType.ArmoryWrist, this) },
            { InventoryType.ArmoryRings, new InventoryGrid(InventoryType.ArmoryRings, this) },
            { InventoryType.ArmorySoulCrystal, new InventoryGrid(InventoryType.ArmorySoulCrystal, this) }
        };
    }

    public override void SortAll()
    {
        foreach (var (key, inventory) in armoryTabs)
        {
            SortController.SortInventory(key, inventory);
        }
    }

    public override void Update()
    {
        foreach (var (key, inventory) in armoryTabs)
        {
            var inventoryCount = InventoryController.GetInventoryItemCount(key);

            if (lastItemCounts.TryAdd(key, inventoryCount)) continue;

            if (lastItemCounts[key] != inventoryCount)
            {
                SortController.SortInventory(key, inventory);
                lastItemCounts[key] = inventoryCount;
            }
        }
    }

    public override void DrawInventoryGrid()
    {
        var region = ImGui.GetContentRegionAvail();

        var selectedGrid = armoryTabs[selectedTab];
        var position = new Vector2(region.X / 2.0f - selectedGrid.InventorySize.X / 2.0f, region.Y / 8.0f);

        selectedGrid.Draw(position);

        var leftBarPosition = position - new Vector2(90.0f, 0.0f) * (selectedGrid.Scale + 0.25f);
        DrawLeftTabBar(leftBarPosition, selectedGrid.Scale + 0.25f);

        var rightBarPosition = new Vector2(position.X + selectedGrid.InventorySize.X + 5.0f * (selectedGrid.Scale + 0.25f), position.Y);
        DrawRightTabBar(rightBarPosition, selectedGrid.Scale + 0.25f);
    }

    private void DrawLeftTabBar(Vector2 drawPosition, float scale)
    {
        DrawTabBar(leftTabTextures, drawPosition, scale);
    }

    private void DrawRightTabBar(Vector2 drawPosition, float scale)
    {
        DrawTabBar(rightTabTextures, drawPosition, scale);
    }

    private void DrawTabBar(Dictionary<InventoryType, TextureWrap?> textures, Vector2 drawPosition, float scale)
    {
        var itemSpacing = new Vector2(0.0f, 85.0f) * scale;

        var index = 0;
        foreach (var (tab, texture) in textures)
        {
            if (texture is null) continue;
            var textureSize = new Vector2(texture.Width, texture.Height) * scale;

            var inactiveColor = Vector4.One with { W = 0.33f };
            var activeColor = Vector4.One;

            if (tab == selectedTab)
            {
                var windowPosition = ImGui.GetWindowPos();

                var borderColor = ImGui.GetColorU32(KnownColor.White.AsVector4());
                var backgroundColor = ImGui.GetColorU32(ImGuiCol.TabActive);

                var rectStart = windowPosition + drawPosition + itemSpacing * index;
                var rectStop = rectStart + textureSize;

                ImGui.GetWindowDrawList().AddRectFilled(rectStart, rectStop, backgroundColor, 5.0f);
                ImGui.GetWindowDrawList().AddRect(rectStart, rectStop, borderColor, 5.0f);
            }

            ImGui.SetCursorPos(drawPosition + itemSpacing * index++);
            ImGui.Image(texture.ImGuiHandle, textureSize, Vector2.Zero, Vector2.One, selectedTab == tab ? activeColor : inactiveColor);

            if (ImGui.IsItemClicked())
            {
                selectedTab = tab;
            }
        }
    }
}