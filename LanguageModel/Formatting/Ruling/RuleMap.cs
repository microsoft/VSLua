/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using LanguageService.Formatting.Options;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleMap
    {
        internal Dictionary<SyntaxKind, Dictionary<SyntaxKind, RuleBucket>> Map;
        private static readonly int Length = Enum.GetNames(typeof(SyntaxKind)).Length;

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
            foreach (SyntaxKind typeLeft in rule.RuleDescriptor.TokenRangeLeft)
            {
                foreach (SyntaxKind typeRight in rule.RuleDescriptor.TokenRangeRight)
                {
                    RuleBucket bucket;
                    Dictionary<SyntaxKind, RuleBucket> leftTokenMap;

                    if (!this.Map.TryGetValue(typeLeft, out leftTokenMap))
                    {
                        this.Map[typeLeft] = new Dictionary<SyntaxKind, RuleBucket>();
                        this.Map[typeLeft][typeRight] = bucket = new RuleBucket();
                    }
                    else
                    {
                        // if this is true, a bucket has been found, and can leave the else safely
                        if (!leftTokenMap.TryGetValue(typeRight, out bucket))
                    {
                            this.Map[typeLeft][typeRight] = bucket = new RuleBucket();
                        }
                    }

                    bucket.Add(rule);
                }
            }
        }

        internal Rule Get(FormattingContext formattingContext)
        {
            SyntaxKind typeLeft = formattingContext.CurrentToken.Token.Kind;
            SyntaxKind typeRight = formattingContext.NextToken.Token.Kind;

            RuleBucket ruleBucket;
            Dictionary<SyntaxKind, RuleBucket> leftTokenMap;

            if (this.Map.TryGetValue(typeLeft, out leftTokenMap))
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

            this.Map = new Dictionary<SyntaxKind, Dictionary<SyntaxKind, RuleBucket>>();
            foreach (Rule rule in Rules.AllRules)
            {
                if (!optionalRuleMap.DisabledRules.Contains(rule))
                {
                    this.Add(rule);
                }
            }
        }
    }
}
