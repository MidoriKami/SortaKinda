using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using ImGuiScene;
using KamiLib.Utilities;
using SortaKinda.Abstracts;
using SortaKinda.Models;
using SortaKinda.Models.Enum;

namespace SortaKinda.System.Modules;

public class ArmoryInventoryModule : InventoryModuleBase
{
    public override ModuleName ModuleName { get; protected set; } = ModuleName.ArmoryInventory;
    public override IInventoryConfig ModuleConfig { get; protected set; } = new ArmoryInventoryConfig();

    private ArmoryTab selectedTab = ArmoryTab.MainHand;

    private readonly Dictionary<ArmoryTab, InventoryGrid> armoryTabs = new()
    {
        { ArmoryTab.MainHand, new InventoryGrid(InventoryType.ArmoryMainHand) },
        { ArmoryTab.OffHand, new InventoryGrid(InventoryType.ArmoryOffHand) },
        { ArmoryTab.Head, new InventoryGrid(InventoryType.ArmoryHead) },
        { ArmoryTab.Body, new InventoryGrid(InventoryType.ArmoryBody) },
        { ArmoryTab.Hands, new InventoryGrid(InventoryType.ArmoryHands) },
        { ArmoryTab.Legs, new InventoryGrid(InventoryType.ArmoryLegs) },
        { ArmoryTab.Feet, new InventoryGrid(InventoryType.ArmoryFeets) },
        { ArmoryTab.Ear, new InventoryGrid(InventoryType.ArmoryEar) },
        { ArmoryTab.Neck, new InventoryGrid(InventoryType.ArmoryNeck) },
        { ArmoryTab.Wrist, new InventoryGrid(InventoryType.ArmoryWrist) },
        { ArmoryTab.Ring, new InventoryGrid(InventoryType.ArmoryRings) },
        { ArmoryTab.SoulGem, new InventoryGrid(InventoryType.ArmorySoulCrystal) },
    };

    private readonly UldWrapper armouryBoard;
    private readonly Dictionary<ArmoryTab, TextureWrap?> leftTabTextures = new();
    private readonly Dictionary<ArmoryTab, TextureWrap?> rightTabTextures = new();

    public ArmoryInventoryModule()
    {
        armouryBoard = Service.PluginInterface.UiBuilder.LoadUld("ui/uld/ArmouryBoard.uld");

        leftTabTextures.Add(ArmoryTab.MainHand, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 0));
        leftTabTextures.Add(ArmoryTab.Head, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 1));
        leftTabTextures.Add(ArmoryTab.Body, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 2));
        leftTabTextures.Add(ArmoryTab.Hands, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 3));
        leftTabTextures.Add(ArmoryTab.Legs, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 5));
        leftTabTextures.Add(ArmoryTab.Feet, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 6));
        
        rightTabTextures.Add(ArmoryTab.OffHand, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 7));
        rightTabTextures.Add(ArmoryTab.Ear, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 8));
        rightTabTextures.Add(ArmoryTab.Neck, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 9));
        rightTabTextures.Add(ArmoryTab.Wrist, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 10));
        rightTabTextures.Add(ArmoryTab.Ring, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 11));
        rightTabTextures.Add(ArmoryTab.SoulGem, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 12));
    }

    public override void Dispose()
    {
        foreach (var loadedTexture in leftTabTextures)
        {
            loadedTexture.Value?.Dispose();
        }
        
        foreach (var loadedTexture in rightTabTextures)
        {
            loadedTexture.Value?.Dispose();
        }
        
        armouryBoard.Dispose();
        
        base.Dispose();
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
        => DrawTabBar(leftTabTextures, drawPosition, scale);

    private void DrawRightTabBar(Vector2 drawPosition, float scale)
        => DrawTabBar(rightTabTextures, drawPosition, scale);
    
    private void DrawTabBar(Dictionary<ArmoryTab, TextureWrap?> textures, Vector2 drawPosition, float scale)
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