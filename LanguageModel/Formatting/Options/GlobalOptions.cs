using System.Collections.Immutable;
using Validation;

namespace LanguageService.Formatting.Options
{

    internal class GlobalOptions
    {
        internal GlobalOptions(NewOptions options)
        {
            Requires.NotNull(options, nameof(options));
            IndentSize = options.IndentSize;
            IndentStyleInfo = options.IndentStyleInfo;
            OptionalRuleMap = new OptionalRuleMap(options.RuleGroupsToDisable);
        }

        internal GlobalOptions()
        {
            IndentSize = 4;
            IndentStyleInfo = new IndentStyleInfo(IndentStyle.Fixed, IndentStyle.Fixed);
            OptionalRuleMap = new OptionalRuleMap(ImmutableArray.Create<OptionalRuleGroup>());
        }

        internal uint IndentSize { get; }
        internal IndentStyleInfo IndentStyleInfo { get; }
        internal OptionalRuleMap OptionalRuleMap { get; }
    }
}
