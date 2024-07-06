using Dalamud.Plugin;
using SortaKinda.Controllers;

namespace SortaKinda;

public sealed class SortaKindaPlugin : IDalamudPlugin {
    public static SortaKindaController Controller = null!;

    public SortaKindaPlugin(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Service>();
        
        Controller = new SortaKindaController();
    }

    public void Dispose() {
        Controller.Dispose();
    }
}