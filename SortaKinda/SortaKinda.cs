using Dalamud.Plugin;
using KamiLib;
using KamiLib.Commands;
using SortaKinda.System;
using SortaKinda.System.Localization;
using SortaKinda.Views.Windows;

namespace SortaKinda;

public sealed class SortaKindaPlugin : IDalamudPlugin
{
    public static SortaKindaSystem System = null!;

    public SortaKindaPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        KamiCommon.Initialize(pluginInterface, Name);
        KamiCommon.RegisterLocalizationHandler(key => Strings.ResourceManager.GetString(key, Strings.Culture));

        System = new SortaKindaSystem();

        CommandController.RegisterMainCommand("/sortakinda", "/sorta");

        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
    }
    public string Name => "SortaKinda";

    public void Dispose()
    {
        KamiCommon.Dispose();

        System.Dispose();
    }
}