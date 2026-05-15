using System.Collections.Generic;
using SortaKinda.OrderRules;

namespace SortaKinda.Windows;

public class OrderingSelectWindow() : SelectionWindowBase<OrderingRuleBase>("Ordering Rule Selection") {

	protected override List<OrderingRuleBase> OptionsList => System.OrderingRules;

	protected override string GetLabelForOption(OrderingRuleBase option)
		=> option.Label;
}