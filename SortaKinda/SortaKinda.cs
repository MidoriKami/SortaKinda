using Dalamud.Plugin;
using SortaKinda.System;

namespace SortaKinda;

public sealed class SortaKindaPlugin : IDalamudPlugin {
    public static SortaKindaController Controller = null!;

    public SortaKindaPlugin(DalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Service>();
        
        Controller = new SortaKindaController();
    }

    public void Dispose() {
        Controller.Dispose();
    }
}