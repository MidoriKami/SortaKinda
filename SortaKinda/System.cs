using KamiLib.CommandManager;
using KamiLib.Window;
using SortaKinda.Classes;
using SortaKinda.Controllers;

namespace SortaKinda;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public static class System {
	public static ModuleController ModuleController { get; set; }
	public static SortController SortController { get; set; }
	public static SystemConfig SystemConfig { get; set; }
	public static SortingThreadController SortingThreadController { get; set; }
	public static CommandManager CommandManager { get; set; }
	public static WindowManager WindowManager { get; set; }
}