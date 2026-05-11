namespace SortaKinda.Configuration;

/// <summary>
/// A singular sorting rule.
/// </summary>
public record struct RuleSet() {
	public string Name = "Name Not Set";
	public bool AllowItem = true;
	public int Priority = 0;
}