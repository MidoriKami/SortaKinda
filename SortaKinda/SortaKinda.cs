using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using KamiToolKit;
using SortaKinda.AddonControllers;
using SortaKinda.Classes;
using SortaKinda.Configuration;
using SortaKinda.FilterRules;
using SortaKinda.OrderRules;
using SortaKinda.Windows;

namespace SortaKinda;

public sealed class SortaKinda : IAsyncDalamudPlugin {

	public SortaKinda(IDalamudPluginInterface pluginInterface) {
		pluginInterface.Create<Services>();
	}

	public Task LoadAsync(CancellationToken cancellationToken) {
		KamiToolKitLibrary.Initialize(Services.PluginInterface, "SortaKinda");

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

		Services.CommandManager.AddHandler("/sortakinda", new CommandInfo(OnCommand) {
			ShowInHelp = true,
			HelpMessage = "Open SortaKinda Config",
		});

		Services.CommandManager.AddHandler("/sorta", new CommandInfo(OnCommand) {
			ShowInHelp = true,
			HelpMessage = "Open SortaKinda Config",
		});

		System.SortingButtonController = new SortingButtonController();

		Services.PluginInterface.UiBuilder.Draw += System.WindowSystem.Draw;
		Services.PluginInterface.UiBuilder.OpenConfigUi += System.ConfigWindow.Toggle;
		Services.PluginInterface.UiBuilder.OpenMainUi += System.ConfigWindow.Toggle;

		Services.ClientState.Login += OnLogin;
		Services.ClientState.Logout += OnLogout;

		if (Services.ClientState.IsLoggedIn) {
			OnLogin();
			System.ConfigWindow.DebugOpen();
		}

		return Task.CompletedTask;
	}

	public async ValueTask DisposeAsync() {
		try {
			Services.ClientState.Login -= OnLogin;
			Services.ClientState.Logout -= OnLogout;

			Services.PluginInterface.UiBuilder.Draw -= System.WindowSystem.Draw;
			Services.PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigWindow.Toggle;
			Services.PluginInterface.UiBuilder.OpenMainUi -= System.ConfigWindow.Toggle;

			System.SortingButtonController.Dispose();

			Services.CommandManager.RemoveHandler("/sortakinda");
			Services.CommandManager.RemoveHandler("/sorta");

            System.WindowSystem.RemoveAllWindows();

            System.SortingController.Dispose();

            await Services.Framework.RunOnFrameworkThread(KamiToolKitLibrary.Dispose);
		}
		catch (Exception exception) {
			Services.PluginLog.Error(exception, "Exception during Async Dispose of SortaKinda.");
		}
	}

	private void OnLogin() {
		System.CharacterConfiguration = CharacterConfiguration.Load();

		// Purge any configurations that are invalid, or can't link to a ruleset.
		var anyPurged = false;
		foreach (var (_, inventoryConfig) in System.CharacterConfiguration.Inventories) {
			var numRemoved = inventoryConfig.SlotSets
				.RemoveAll(set => set.RuleSetId == Guid.Empty || System.SystemConfiguration.RuleSets
				    .All(systemSet => systemSet.RuleSetId != set.RuleSetId));

			if (numRemoved > 0) {
				anyPurged = true;
			}
		}

		if (anyPurged) {
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