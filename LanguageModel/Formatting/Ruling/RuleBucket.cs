using System.Collections.Generic;

namespace LanguageModel.Formatting.Ruling
{
    internal class RuleBucket
    {
        private List<Rule> rules;

        // The reason I am wrapping a class around the List collections is because
        // I might want to order the rules... And it looks like I might have to
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
                if (rule.Applies(formattingContext))
                {
                    return rule;
                }
            }
            return null;
        }


    }
}
