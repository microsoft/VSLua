/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using Validation;

namespace LanguageService.Formatting.Ruling
{
    internal class SimpleRule : Rule
    {
        internal SimpleRule(RuleDescriptor ruleDescriptor, List<Func<FormattingContext, bool>> contextFilters, RuleAction action)
        {
            Requires.NotNull(ruleDescriptor, nameof(ruleDescriptor));
            Requires.NotNull(contextFilters, nameof(contextFilters));

            this.RuleDescriptor = ruleDescriptor;
            this.RuleOperationContext = new RuleOperation(new RuleOperationContext(contextFilters), action);
        }

        internal override RuleDescriptor RuleDescriptor { get; }

        internal override RuleOperation RuleOperationContext { get; }

        internal override bool AppliesTo(FormattingContext formattingContext)
        {
            return this.RuleOperationContext.Context.InContext(formattingContext);
        }

        internal override IEnumerable<TextEditInfo> Apply(FormattingContext formattingContext)
        {
            Token leftToken = formattingContext.CurrentToken.Token;
            Token rightToken = formattingContext.NextToken.Token;

            int start = leftToken.Start + leftToken.Text.Length;
            int length = rightToken.Start - start;
            string replaceWith = this.GetTextFromAction();

            return new List<TextEditInfo> { new TextEditInfo(start, length, replaceWith) };
        }

        private string GetTextFromAction()
        {
            switch (this.RuleOperationContext.Action)
            {
                case RuleAction.Delete:
                    return string.Empty;
                case RuleAction.Newline:
                    return "\n";
                case RuleAction.Space:
                    return " ";
                default:
                    return string.Empty;
            }
        }
    }
}
