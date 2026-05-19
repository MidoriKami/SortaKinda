using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace SortaKinda;

public class Services {
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; set; } = null!;
    [PluginService] public static IReliableFileStorage ReliableFileStorage { get; set; } = null!;
    [PluginService] public static IFramework Framework { get; set; } = null!;
    [PluginService] public static IClientState ClientState { get; set; } = null!;
    [PluginService] public static IPlayerState PlayerState { get; set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; set; } = null!;
    [PluginService] public static ITextureProvider TextureProvider { get; set; } = null!;
    [PluginService] public static IDataManager DataManager { get; set; } = null!;
    [PluginService] public static IKeyState KeyState { get; set; } = null!;
    [PluginService] public static IGameInventory GameInventory { get; set; } = null!;
}