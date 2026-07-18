using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using SortaKinda.Classes;
using SortaKinda.Configuration;
using SortaKinda.FilterRules;
using SortaKinda.OrderRules;
using SortaKinda.Windows;

namespace SortaKinda;

public sealed class SortaKinda : IAsyncDalamudPlugin {
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; set; } = null!;

	public Task LoadAsync(CancellationToken cancellationToken) {
		System.SystemConfiguration = SystemConfiguration.Load();
		System.SortingController = new SortingController();

		System.ConfigWindow = new ConfigWindow();

		System.OrderingRuleTypes =
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(type => type.IsSubclassOf(typeof(OrderingRuleBase)))
			        .Where(type => !type.IsAbstract)
			        .ToList();

		System.FilteringRuleTypes =
			Assembly.GetExecutingAssembly()
			        .GetTypes()
			        .Where(type => type.IsSubclassOf(typeof(FilteringRuleBase)))
			        .Where(type => !type.IsAbstract)
			        .ToList();

		System.WindowSystem = new WindowSystem("ChillFrames");
		System.WindowSystem.AddWindow(System.ConfigWindow);

		ICommandManager.Get().AddHandler("/sortakinda", new CommandInfo(OnCommand) {
			ShowInHelp = true,
			HelpMessage = "Open SortaKinda Config",
		});

		ICommandManager.Get().AddHandler("/sorta", new CommandInfo(OnCommand) {
			ShowInHelp = true,
			HelpMessage = "Open SortaKinda Config",
		});

		PluginInterface.UiBuilder.Draw += System.WindowSystem.Draw;
		PluginInterface.UiBuilder.OpenConfigUi += System.ConfigWindow.Toggle;
		PluginInterface.UiBuilder.OpenMainUi += System.ConfigWindow.Toggle;

		IClientState.Get().Login += OnLogin;
		IClientState.Get().Logout += OnLogout;

		if (IClientState.Get().IsLoggedIn) {
			OnLogin();
			System.ConfigWindow.DebugOpen();
		}

		return Task.CompletedTask;
	}

	public ValueTask DisposeAsync() {
		try {
			IClientState.Get().Login -= OnLogin;
			IClientState.Get().Logout -= OnLogout;

			PluginInterface.UiBuilder.Draw -= System.WindowSystem.Draw;
			PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigWindow.Toggle;
			PluginInterface.UiBuilder.OpenMainUi -= System.ConfigWindow.Toggle;

			ICommandManager.Get().RemoveHandler("/sortakinda");
			ICommandManager.Get().RemoveHandler("/sorta");

            System.WindowSystem.RemoveAllWindows();

            System.SortingController.Dispose();

			return ValueTask.CompletedTask;
		}
		catch (Exception exception) {
			return ValueTask.FromException(exception);
		}
	}

	private void OnLogin() {
		System.CharacterConfiguration = CharacterConfiguration.Load();

		if (System.CharacterConfiguration.PurgeInvalidSlotSets()) {
			System.CharacterConfiguration.Save();
		}

		System.SortingController.OnLogin();
	}

	private void OnLogout(int type, int code) {
		System.CharacterConfiguration = null;
	}

	private static void OnCommand(string command, string arguments) {
		if (command is not ("/sortakinda" or "/sorta")) return;

		switch (arguments.Split(' ')) {
			case [ "" ] or []:
				System.ConfigWindow.Toggle();
				return;

			case [ "sort" ]:
				System.SortingController.LaunchSortTask();
				return;
		}
	}
}