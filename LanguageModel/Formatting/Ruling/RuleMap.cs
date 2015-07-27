using System;
using System.Collections.Generic;
using LanguageService.Formatting.Options;

namespace LanguageService.Formatting.Ruling
{

    internal static class RuleMap
    {
        internal static RuleBucket[,] map;
        private static int Length { get; }

        static RuleMap()
        {
            Length = Enum.GetNames(typeof(TokenType)).Length;
            ClearRuleMapAndAddEnabledRules(null);
        }

        internal static void Add(Rule rule)
        {
            foreach (TokenType typeLeft in rule.RuleDescriptor.TokenRangeLeft)
            {
                foreach (TokenType typeRight in rule.RuleDescriptor.TokenRangeRight)
                {
                    int column = (int)typeLeft;
                    int row = (int)typeRight;

                    RuleBucket bucket = map[column, row];
                    if (bucket == null)
                    {
                        bucket = new RuleBucket();
                    }
                    bucket.Add(rule);
                    map[column, row] = bucket;
                }
            }
        }

        internal static Rule Get(FormattingContext formattingContext)
        {
            int column = (int)formattingContext.CurrentToken.Token.Type;
            int row = (int)formattingContext.NextToken.Token.Type;

            RuleBucket ruleBucket = map[column, row];

            if (ruleBucket != null)
            {
                return ruleBucket.Get(formattingContext);
            }
            return null;
        }

        internal static void ClearRuleMapAndAddEnabledRules(OptionalRuleMap optionalRuleMap)
        {
            if (optionalRuleMap == null)
            {
                optionalRuleMap = new OptionalRuleMap(new List<OptionalRuleGroup>());
            }

            map = new RuleBucket[Length, Length];
            foreach (Rule rule in Rules.AllRules)
            {
                if (!optionalRuleMap.DisabledRules.Contains(rule))
                {
                    Add(rule);
                }
            }
        }
    }
}
