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
    private ConfigurationWindow window;
    
    public SortaKindaPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        
        KamiCommon.Initialize(pluginInterface, Name);
        KamiCommon.RegisterLocalizationHandler(key => Strings.ResourceManager.GetString(key, Strings.Culture));
                
        System = new SortaKindaSystem();
        System.Load();

        CommandController.RegisterMainCommand("/sortakinda");

        window = new ConfigurationWindow();
        KamiCommon.WindowManager.AddConfigurationWindow(window);
    }

    public void Dispose()
    {
        KamiCommon.Dispose();
        
        System.Unload();
        window.Dispose();
    }
}