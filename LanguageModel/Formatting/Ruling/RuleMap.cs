using System;
using System.Collections.Generic;
using System.Linq;
using LanguageService.Formatting.Options;

namespace LanguageService.Formatting.Ruling
{

    internal class RuleMap
    {
        internal RuleBucket[,] map;
        private static readonly int Length = Enum.GetNames(typeof(TokenType)).Length;

        internal static RuleMap Create(OptionalRuleMap optionalRuleMap)
        {
            RuleMap ruleMap = new RuleMap();
            ruleMap.AddEnabledRules(optionalRuleMap);

            return ruleMap;
        }

        internal static RuleMap Create()
        {
            RuleMap ruleMap = new RuleMap();
            ruleMap.AddEnabledRules(new OptionalRuleMap(Enumerable.Empty<OptionalRuleGroup>()));

            return ruleMap;
        }

        internal void Add(Rule rule)
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

        internal Rule Get(FormattingContext formattingContext)
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

        private void AddEnabledRules(OptionalRuleMap optionalRuleMap)
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
