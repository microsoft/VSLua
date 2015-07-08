using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting.Ruling
{

    internal static class RuleMap
    {
        internal static RuleBucket[,] map;

        internal static void Clear()
        {
            if (map != null)
            {
                map = null;
            }

            int length = Enum.GetNames(typeof(TokenType)).Length;

            RuleMap.map = new RuleBucket[length, length];

        }

        internal static void AddRule(Rule rule)
        {
            foreach (TokenType typeLeft in rule.ruleDescriptor.TokenRangeLeft)
            {
                foreach (TokenType typeRight in rule.ruleDescriptor.TokenRangeRight)
                {
                    int column = (int)typeLeft;
                    int row = (int)typeRight;

                    RuleBucket bucket = RuleMap.map[column, row];
                    if (bucket == null)
                    {
                        bucket = new RuleBucket();
                    }

                    bucket.Add(rule);

                }
            }
        }

        private static Rule GetRule(FormattingContext formattingContext)
        {
            int column = (int)formattingContext.CurrentToken.Token.Type;
            int row = (int)formattingContext.NextToken.Token.Type;

            RuleBucket ruleBucket = RuleMap.map[column,row];
            
            if (ruleBucket != null)
            {
                return ruleBucket.Get(formattingContext);
            }

            return null;

        }


    }
}
