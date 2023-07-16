using Dalamud.Plugin;
using KamiLib;
using KamiLib.Commands;
using SortaKinda.System;
using SortaKinda.System.Localization;
using SortaKinda.Views.Windows;

namespace SortaKinda;

public sealed class SortaKindaPlugin : IDalamudPlugin
{
    public string Name => "SortaKinda";

    public static SortaKindaSystem System = null!;
    
    public SortaKindaPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        
        KamiCommon.Initialize(pluginInterface, Name);
        KamiCommon.RegisterLocalizationHandler(key => Strings.ResourceManager.GetString(key, Strings.Culture));
                
        System = new SortaKindaSystem();

        CommandController.RegisterMainCommand("/sortakinda");

        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
    }

    public void Dispose()
    {
        KamiCommon.Dispose();
        
        System.Dispose();
    }
}