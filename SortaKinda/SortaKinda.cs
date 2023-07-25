using Dalamud.Plugin;
using KamiLib;
using KamiLib.Commands;
using SortaKinda.Controllers.Localization;
using SortaKinda.System;
using SortaKinda.Views.Windows;

namespace SortaKinda;

public sealed class SortaKindaPlugin : IDalamudPlugin
{
    public string Name => "SortaKinda";

    public static SortaKindaController Controller = null!;

    public SortaKindaPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        KamiCommon.Initialize(pluginInterface, Name);
        KamiCommon.RegisterLocalizationHandler(key => Strings.ResourceManager.GetString(key, Strings.Culture));

        Controller = new SortaKindaController();

        CommandController.RegisterMainCommand("/sortakinda", "/sorta");

        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
    }

    public void Dispose()
    {
        KamiCommon.Dispose();

        Controller.Dispose();
    }
}