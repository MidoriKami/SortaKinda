using System.Diagnostics;
using Dalamud.Interface.Windowing;

namespace SortaKinda.Extensions;

/// <summary>
/// Debug helpers so that I can have the windows open automatically,
/// but also not do that when I inevitably forget to remove the call to them when releasing this update.
/// </summary>
public static class WindowExtensions {
	extension(IWindow window) {
		[Conditional("DEBUG")]
		public void DebugOpen()
			=> window.IsOpen = true;
	}
}