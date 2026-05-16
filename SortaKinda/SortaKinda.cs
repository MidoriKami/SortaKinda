using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
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
		System.SystemConfiguration = SystemConfiguration.Load();

		System.ConfigWindow = new ConfigWindow();

		System.FilteringRules = Assembly
	         .GetExecutingAssembly()
	         .GetTypes()
	         .Where(type => type.IsSubclassOf(typeof(FilteringRuleBase)))
	         .Where(type => !type.IsAbstract)
	         .Select(type => (FilteringRuleBase?)Activator.CreateInstance(type))
	         .OfType<FilteringRuleBase>()
	         .OrderBy(rule => rule.Label)
	         .ToList();

		System.OrderingRules = Assembly
			.GetExecutingAssembly()
			.GetTypes()
			.Where(type => type.IsSubclassOf(typeof(OrderingRuleBase)))
			.Where(type => !type.IsAbstract)
			.Select(type => (OrderingRuleBase?)Activator.CreateInstance(type))
			.OfType<OrderingRuleBase>()
			.OrderBy(rule => rule.Label)
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

		Services.PluginInterface.UiBuilder.Draw += System.WindowSystem.Draw;
		Services.PluginInterface.UiBuilder.OpenConfigUi += System.ConfigWindow.Toggle;
		Services.PluginInterface.UiBuilder.OpenMainUi += System.ConfigWindow.Toggle;

		Services.ClientState.Login += OnLogin;
		Services.ClientState.Logout += OnLogout;

		if (Services.ClientState.IsLoggedIn) {
			OnLogin();
		}

		System.ConfigWindow.DebugOpen();

		return Task.CompletedTask;
	}

	public ValueTask DisposeAsync() {
		try {
			Services.ClientState.Login -= OnLogin;
			Services.ClientState.Logout -= OnLogout;

			Services.PluginInterface.UiBuilder.Draw -= System.WindowSystem.Draw;
			Services.PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigWindow.Toggle;
			Services.PluginInterface.UiBuilder.OpenMainUi -= System.ConfigWindow.Toggle;

			Services.CommandManager.RemoveHandler("/sortakinda");
			Services.CommandManager.RemoveHandler("/sorta");

            System.WindowSystem.RemoveAllWindows();

			return ValueTask.CompletedTask;
		}
		catch (Exception exception) {
			return ValueTask.FromException(exception);
		}
	}

	private void OnLogin() {
		System.CharacterConfiguration = CharacterConfiguration.Load();
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
		}
	}
}