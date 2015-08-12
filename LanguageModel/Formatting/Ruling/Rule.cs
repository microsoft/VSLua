/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System.Collections.Generic;

namespace LanguageService.Formatting.Ruling
{
    internal abstract class Rule
    {
        internal abstract RuleDescriptor RuleDescriptor { get; }

        internal abstract RuleOperation RuleOperationContext { get; }

        internal abstract bool AppliesTo(FormattingContext formattingContext);

        internal abstract IEnumerable<TextEditInfo> Apply(FormattingContext formattingContext);
    }
}
