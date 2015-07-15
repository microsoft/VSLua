using System.Collections.Generic;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleBucket
    {
        private List<Rule> rules;

        // This methods is here for when I need to add precedence to the rules
        internal void Add(Rule rule)
        {
            if (rules == null)
            {
                rules = new List<Rule>();
            }
            rules.Add(rule);
        }

        internal Rule Get(FormattingContext formattingContext)
        {
            foreach (Rule rule in rules)
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
