using System.Collections.Generic;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleBucket
    {
        private List<IRule> rules;

        // The reason I am wrapping a class around the List collections is because
        // I might want to order the rules... And it looks like I might have to
        internal void Add(IRule rule)
        {
            if (rules == null)
            {
                rules = new List<IRule>();
            }
            rules.Add(rule);
        }

        internal IRule Get(FormattingContext formattingContext)
        {
            foreach (IRule rule in rules)
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
