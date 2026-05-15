using System.Collections.Generic;
using SortaKinda.FilterRules;

namespace SortaKinda.Windows;

public class FilterSelectWindow() : SelectionWindowBase<FilteringRuleBase>("Filtering Rule Selection") {

	protected override List<FilteringRuleBase> OptionsList => System.FilteringRules;

	protected override string GetLabelForOption(FilteringRuleBase option)
		=> option.Label;
}