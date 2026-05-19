using SortaKinda.OrderRules;

namespace SortaKinda.Windows;

public class OrderingSelectWindow() : SelectionWindowBase<OrderingRuleBase>("Ordering Rule Selection") {

	protected override string GetLabelForOption(OrderingRuleBase option)
		=> option.Label;
}