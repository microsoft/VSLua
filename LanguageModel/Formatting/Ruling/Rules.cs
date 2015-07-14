using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class Rules
    {

        private static readonly List<ContextFilter> defaultFilters = new List<ContextFilter>
        {
            TokensAreOnSameLine,
            NoCommentBetweenTokens
        };

        internal static AbstractRule SpaceAfterComma =
            new Rule(
                new RuleDescriptor(TokenType.Comma, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceAfterAssignmentOperator =
            new Rule(
                new RuleDescriptor(TokenType.AssignmentOperator, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceBeforeAssignmentOperator =
            new Rule(
                new RuleDescriptor(TokenRange.AnyVisible, TokenType.AssignmentOperator),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceAfterBinaryOperator =
            new Rule(
                new RuleDescriptor(TokenRange.BinaryOperators, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceBeforeBinaryOperator =
            new Rule(
                new RuleDescriptor(TokenRange.AnyVisible, TokenRange.BinaryOperators),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceAfterValueBeforeOpenParenthesis =
            new Rule(
                new RuleDescriptor(TokenRange.Value, TokenType.OpenParen),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceBeforeValueAfterOpenParenthesis =
            new Rule(
                new RuleDescriptor(TokenType.OpenParen, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceBeforeValueAfterOpenSquareBracket =
            new Rule(
                new RuleDescriptor(TokenType.OpenBracket, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceBeforeValueAfterOpenCurlyBrace =
            new Rule(new RuleDescriptor(TokenType.OpenCurlyBrace, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceAfterValueBeforeCloseParenthesis =
            new Rule(new RuleDescriptor(TokenRange.Value, TokenType.CloseParen),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceAfterValueBeforeCloseSquareBracket =
            new Rule(new RuleDescriptor(TokenRange.Value, TokenType.CloseBracket),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule SpaceAfterValueBeforeCloseCurlyBrace =
            new Rule(new RuleDescriptor(TokenRange.Value, TokenType.CloseCurlyBrace),
                defaultFilters, RuleAction.Space);

        internal static AbstractRule DeleteSpaceBeforeEofToken =
            new Rule(new RuleDescriptor(TokenRange.AnyVisible, TokenType.EndOfFile),
                defaultFilters, RuleAction.Delete);

        internal static AbstractRule DeleteTrailingWhitespace = new RuleTrailingWhitespace();

        internal static RuleMap GetRuleMap()
        {
            RuleMap ruleMap = new RuleMap();
            ruleMap.AddRule(Rules.SpaceAfterComma);
            ruleMap.AddRule(Rules.SpaceAfterAssignmentOperator);
            ruleMap.AddRule(Rules.SpaceAfterBinaryOperator);
            ruleMap.AddRule(Rules.SpaceAfterValueBeforeCloseCurlyBrace);
            ruleMap.AddRule(Rules.SpaceAfterValueBeforeCloseParenthesis);
            ruleMap.AddRule(Rules.SpaceAfterValueBeforeCloseSquareBracket);
            ruleMap.AddRule(Rules.SpaceAfterValueBeforeOpenParenthesis);
            ruleMap.AddRule(Rules.SpaceBeforeAssignmentOperator);
            ruleMap.AddRule(Rules.SpaceBeforeBinaryOperator);
            ruleMap.AddRule(Rules.SpaceBeforeValueAfterOpenCurlyBrace);
            ruleMap.AddRule(Rules.SpaceBeforeValueAfterOpenParenthesis);
            ruleMap.AddRule(Rules.SpaceBeforeValueAfterOpenSquareBracket);
            ruleMap.AddRule(Rules.DeleteTrailingWhitespace);
            ruleMap.AddRule(Rules.DeleteSpaceBeforeEofToken);
            return ruleMap;
        }


        internal static bool TokensAreOnSameLine(FormattingContext formattingContext)
        {
            return formattingContext.TokensOnSameLine();
        }

        internal static bool TokensAreNotOnSameLine(FormattingContext formattingContext)
        {
            return !formattingContext.TokensOnSameLine();
        }

        internal static bool NoCommentBetweenTokens(FormattingContext formattingContext)
        {
            return !formattingContext.CommentsInBetween();
        }

    }
}
