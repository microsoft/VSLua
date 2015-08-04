using System;
using System.Collections.Generic;
using System.Linq;
using LanguageService.Formatting.Options;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleMap
    {
        internal Dictionary<TokenType, Dictionary<TokenType, RuleBucket>> map;
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
                    RuleBucket bucket;
                    Dictionary<TokenType, RuleBucket> leftTokenMap;

                    if (!map.TryGetValue(typeLeft, out leftTokenMap))
                    {
                        map[typeLeft] = new Dictionary<TokenType, RuleBucket>();
                        map[typeLeft][typeRight] = bucket = new RuleBucket();
                    }
                    else
                    {
                        // if this is true, a bucket has been found, and can leave the else safely
                        if (!leftTokenMap.TryGetValue(typeRight, out bucket))
                    {
                            map[typeLeft][typeRight] = bucket = new RuleBucket();
                        }
                    }
                    bucket.Add(rule);

                }
            }
        }

        internal Rule Get(FormattingContext formattingContext)
        {
            TokenType typeLeft = formattingContext.CurrentToken.Token.Type;
            TokenType typeRight = formattingContext.NextToken.Token.Type;

            RuleBucket ruleBucket;
            Dictionary<TokenType, RuleBucket> leftTokenMap;

            if (map.TryGetValue(typeLeft, out leftTokenMap))
            {
                if (leftTokenMap.TryGetValue(typeRight, out ruleBucket))
            {
                return ruleBucket.Get(formattingContext);
            }
            }

            return null;
        }

        private void AddEnabledRules(OptionalRuleMap optionalRuleMap)
        {
            if (optionalRuleMap == null)
            {
                optionalRuleMap = new OptionalRuleMap(Enumerable.Empty<OptionalRuleGroup>());
            }

            map = new Dictionary<TokenType, Dictionary<TokenType, RuleBucket>>();
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
