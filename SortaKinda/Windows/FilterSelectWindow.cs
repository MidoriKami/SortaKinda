using SortaKinda.FilterRules;

namespace SortaKinda.Windows;

public class FilterSelectWindow() : SelectionWindowBase<FilteringRuleBase>("Filtering Rule Selection") {

	protected override string GetLabelForOption(FilteringRuleBase option)
		=> option.Label;
}