using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaKinda.Classes;

namespace SortaKinda.ViewComponents;

public class ArmoryInventoryGridView : IDisposable {
    private readonly UldWrapper armouryBoard;
    private readonly Dictionary<InventoryType, IDalamudTextureWrap?> leftTabTextures = new();
    private readonly Dictionary<InventoryType, IDalamudTextureWrap?> rightTabTextures = new();

    private readonly Dictionary<InventoryType, InventoryGridView> views = new();
    private InventoryType selectedTab = InventoryType.ArmoryMainHand;

    private static Vector2 ButtonSize 
        => ImGuiHelpers.ScaledVector2(80.0f, 80.0f) * .75f;
    
    private static Vector2 ButtonPadding 
        => ImGuiHelpers.ScaledVector2(5.0f, 5.0f);

    public ArmoryInventoryGridView(List<InventoryGrid> armoryInventories) {
        var region = ImGuiHelpers.ScaledVector2(404, 565);
        var position = new Vector2(region.X / 2.0f - InventoryGridView.GetGridWidth() / 2.0f, region.Y / 8.0f);

        foreach (var inventory in armoryInventories) {
            views.Add(inventory.Type, new InventoryGridView(inventory, position));
        }

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

    public void Dispose() {
        foreach (var loadedTexture in leftTabTextures) loadedTexture.Value?.Dispose();
        foreach (var loadedTexture in rightTabTextures) loadedTexture.Value?.Dispose();
        armouryBoard.Dispose();
    }

    public void Draw() {
        views[selectedTab].Draw();

        var region = ImGuiHelpers.ScaledVector2(404, 565);
        var gridWidth = InventoryGridView.GetGridWidth();
        var position = new Vector2(region.X / 2.0f - gridWidth / 2.0f, region.Y / 8.0f);

        var leftOffset = ButtonSize.X + ButtonPadding.X;
        var leftBarPosition = position - new Vector2(leftOffset, 0.0f);
        DrawTabBar(leftTabTextures, leftBarPosition);

        var rightOffset = ButtonPadding.X;
        var rightBarPosition = position + new Vector2(gridWidth + rightOffset, 0.0f);
        DrawTabBar(rightTabTextures, rightBarPosition);
    }

    private void DrawTabBar(Dictionary<InventoryType, IDalamudTextureWrap?> textures, Vector2 drawPosition) {
        var itemSpacing = new Vector2(0.0f, ButtonSize.Y + ButtonPadding.Y);

        var index = 0;
        foreach (var (tab, texture) in textures) {
            if (texture is null) continue;
            var inactiveColor = Vector4.One with { W = 0.33f };
            var activeColor = Vector4.One;

            if (tab == selectedTab) {
                var windowPosition = ImGui.GetWindowPos();

                var borderColor = ImGui.GetColorU32(KnownColor.White.Vector());
                var backgroundColor = ImGui.GetColorU32(KnownColor.Gray.Vector() with { W = 0.50f });

                var rectStart = windowPosition + drawPosition + itemSpacing * index;
                var rectStop = rectStart + ButtonSize;

                ImGui.GetWindowDrawList().AddRectFilled(rectStart, rectStop, backgroundColor, 5.0f);
                ImGui.GetWindowDrawList().AddRect(rectStart, rectStop, borderColor, 5.0f);
            }

            ImGui.SetCursorPos(drawPosition + itemSpacing * index++);
            ImGui.Image(texture.Handle, ButtonSize, Vector2.Zero, Vector2.One, selectedTab == tab ? activeColor : inactiveColor);

            if (ImGui.IsItemClicked()) {
                selectedTab = tab;
            }
        }
    }
}