/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System.Collections.Immutable;
using Validation;

namespace LanguageService.Formatting.Options
{
    internal class GlobalOptions
    {
        internal GlobalOptions(NewOptions options)
        {
            Requires.NotNull(options, nameof(options));
            this.IndentSize = options.IndentSize;
            this.TabSize = options.TabSize;
            this.UsingTabs = options.UsingTabs;
            this.OptionalRuleMap = new OptionalRuleMap(options.RuleGroupsToDisable);
        }

        internal GlobalOptions()
        {
            this.IndentSize = 4;
            this.OptionalRuleMap = new OptionalRuleMap(ImmutableArray.Create<OptionalRuleGroup>());
        }

        internal uint IndentSize { get; }
        internal uint TabSize { get; }
        internal bool UsingTabs { get; }
        internal OptionalRuleMap OptionalRuleMap { get; }
    }
}
