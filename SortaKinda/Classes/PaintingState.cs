namespace SortaKinda.Classes;

/// <summary>
/// Enum representing the current state of the area-painting for inventory slot selection.
/// </summary>
public enum PaintingState {

	/// <summary>
	/// Awaiting click-drag
	/// </summary>
	Waiting,

	/// <summary>
	/// Click-drag in progress
	/// </summary>
	Started,

	/// <summary>
	/// Completed, will reset on next frame
	/// </summary>
	Completed,
}