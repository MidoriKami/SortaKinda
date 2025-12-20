using KamiLib.CommandManager;
using KamiLib.Window;
using SortaKinda.Addons;
using SortaKinda.Classes;
using SortaKinda.Controllers;

namespace SortaKinda;

public static class System {
	public static ModuleController ModuleController { get; set; } = null!;
	public static SortController SortController { get; set; } = null!;
	public static SystemConfig SystemConfig { get; set; } = null!;
	public static SortingThreadController SortingThreadController { get; set; } = null!;
	public static CommandManager CommandManager { get; set; } = null!;
	public static WindowManager WindowManager { get; set; } = null!;
	public static AddonControllers AddonControllers { get; set; } = null!;
}