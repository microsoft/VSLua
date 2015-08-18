/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System.Collections.Generic;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleBucket
    {
        private List<Rule> rules;

        // This methods is here for when I need to add precedence to the rules
        internal void Add(Rule rule)
        {
            if (this.rules == null)
            {
                this.rules = new List<Rule>();
            }

            this.rules.Add(rule);
        }

        internal Rule Get(FormattingContext formattingContext)
        {
            foreach (Rule rule in this.rules)
            {
                if (rule.AppliesTo(formattingContext))
                {
                    return rule;
                }
            }

            return null;
        }
    }
}
