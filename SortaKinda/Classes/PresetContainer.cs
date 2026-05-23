using System.Collections.Generic;
using SortaKinda.Configuration;

namespace SortaKinda.Classes;

/// <summary>
/// Class to contain sharable preset data, wraps it in a handy place
/// in case we have to change or break something later.
/// </summary>
public class PresetContainer {
	public int Version = 1;
	public List<RuleSet> RuleSets = [];
}