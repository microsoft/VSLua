using System.Collections.Generic;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleBucket
    {
        private List<AbstractRule> rules;

        // The reason I am wrapping a class around the List collections is because
        // I might want to order the rules... And it looks like I might have to
        internal void Add(AbstractRule rule)
        {
            if (rules == null)
            {
                rules = new List<AbstractRule>();
            }
            rules.Add(rule);
        }

        internal AbstractRule Get(FormattingContext formattingContext)
        {
            foreach (AbstractRule rule in rules)
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
