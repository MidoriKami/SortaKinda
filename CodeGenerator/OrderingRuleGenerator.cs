using Microsoft.CodeAnalysis;

namespace CodeGenerator;

[Generator]
public class OrderingRuleGenerator : JsonMappingGenerator {
	protected override string TargetType => "OrderingRuleBase";
	protected override string BaseNameSpace => "SortaKinda.OrderRules";
}