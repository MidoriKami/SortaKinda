using Microsoft.CodeAnalysis;

namespace CodeGenerator;

[Generator]
public class FilterRuleGenerator : JsonMappingGenerator {
    protected override string TargetType => "FilteringRuleBase";
    protected override string BaseNameSpace => "SortaKinda.FilterRules";
}