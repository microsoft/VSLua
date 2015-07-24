using System;

namespace LanguageService.Formatting.Ruling
{

    internal static class RuleMap
    {
        internal static RuleBucket[,] map;

        static RuleMap()
        {
            int length = Enum.GetNames(typeof(TokenType)).Length;
            map = new RuleBucket[length, length];
            Initialize();
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

        private static void Initialize()
        {
            Add(Rules.SpaceAfterComma);
            Add(Rules.SpaceAfterAssignmentOperator);
            Add(Rules.SpaceAfterBinaryOperator);
            Add(Rules.SpaceAfterValueBeforeCloseCurlyBrace);
            Add(Rules.SpaceAfterValueBeforeCloseParenthesis);
            Add(Rules.SpaceAfterValueBeforeCloseSquareBracket);
            Add(Rules.SpaceAfterValueBeforeOpenParenthesis);
            Add(Rules.SpaceBeforeAssignmentOperator);
            Add(Rules.SpaceBeforeBinaryOperator);
            Add(Rules.SpaceBeforeValueAfterOpenCurlyBrace);
            Add(Rules.SpaceBeforeValueAfterOpenParenthesis);
            Add(Rules.SpaceBeforeValueAfterOpenSquareBracket);
            Add(Rules.DeleteTrailingWhitespace);
            Add(Rules.DeleteSpaceBeforeEofToken);
            Add(Rules.DeleteSpaceAfterValueBeforeColon);
            Add(Rules.DeleteSpaceAfterValueBeforeDot);
            Add(Rules.DeleteSpaceBeforeValueAfterDot);
            Add(Rules.DeleteSpaceBeforeValueAfterColon);
        }
    }
}
