using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class Rules
    {
        private static readonly List<Func<FormattingContext, bool>> defaultFilters = new List<Func<FormattingContext, bool>>
        {
            TokensAreOnSameLine,
            NoCommentsBetweenTokens
        };

        internal static Rule SpaceAfterComma =
            new SimpleRule(
                new RuleDescriptor(TokenType.Comma, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterAssignmentOperator =
            new SimpleRule(
                new RuleDescriptor(TokenType.AssignmentOperator, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeAssignmentOperator =
            new SimpleRule(
                new RuleDescriptor(TokenRange.AnyVisible, TokenType.AssignmentOperator),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterBinaryOperator =
            new SimpleRule(
                new RuleDescriptor(TokenRange.BinaryOperators, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeBinaryOperator =
            new SimpleRule(
                new RuleDescriptor(TokenRange.AnyVisible, TokenRange.BinaryOperators),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeOpenParenthesis =
            new SimpleRule(
                new RuleDescriptor(TokenRange.Value, TokenType.OpenParen),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeValueAfterOpenParenthesis =
            new SimpleRule(
                new RuleDescriptor(TokenType.OpenParen, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeValueAfterOpenSquareBracket =
            new SimpleRule(
                new RuleDescriptor(TokenType.OpenBracket, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeValueAfterOpenCurlyBrace =
            new SimpleRule(new RuleDescriptor(TokenType.OpenCurlyBrace, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeCloseParenthesis =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, TokenType.CloseParen),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeCloseSquareBracket =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, TokenType.CloseBracket),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeCloseCurlyBrace =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, TokenType.CloseCurlyBrace),
                defaultFilters, RuleAction.Space);

        internal static DeleteSpaceBeforeEofToken DeleteSpaceBeforeEofToken = new DeleteSpaceBeforeEofToken();

        internal static Rule DeleteSpaceAfterValueBeforeDot =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, TokenType.Dot),
                defaultFilters, RuleAction.Delete);

        internal static Rule DeleteSpaceBeforeValueAfterDot =
            new SimpleRule(new RuleDescriptor(TokenType.Dot, TokenRange.Value),
                defaultFilters, RuleAction.Delete);

        internal static Rule DeleteSpaceAfterValueBeforeColon =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, TokenType.Colon),
                defaultFilters, RuleAction.Delete);

        internal static Rule DeleteSpaceBeforeValueAfterColon =
            new SimpleRule(new RuleDescriptor(TokenType.Colon, TokenRange.Value),
                defaultFilters, RuleAction.Delete);

        internal static Rule DeleteTrailingWhitespace = new DeleteTrailingWhitespace();

        // This would create the RuleMap off the formatting options, but for now
        //   it just adds all the avaliable rules.
        internal static RuleMap GetRuleMap()
        {
            RuleMap ruleMap = new RuleMap();
            ruleMap.Add(Rules.SpaceAfterComma);
            ruleMap.Add(Rules.SpaceAfterAssignmentOperator);
            ruleMap.Add(Rules.SpaceAfterBinaryOperator);
            ruleMap.Add(Rules.SpaceAfterValueBeforeCloseCurlyBrace);
            ruleMap.Add(Rules.SpaceAfterValueBeforeCloseParenthesis);
            ruleMap.Add(Rules.SpaceAfterValueBeforeCloseSquareBracket);
            ruleMap.Add(Rules.SpaceAfterValueBeforeOpenParenthesis);
            ruleMap.Add(Rules.SpaceBeforeAssignmentOperator);
            ruleMap.Add(Rules.SpaceBeforeBinaryOperator);
            ruleMap.Add(Rules.SpaceBeforeValueAfterOpenCurlyBrace);
            ruleMap.Add(Rules.SpaceBeforeValueAfterOpenParenthesis);
            ruleMap.Add(Rules.SpaceBeforeValueAfterOpenSquareBracket);
            ruleMap.Add(Rules.DeleteTrailingWhitespace);
            ruleMap.Add(Rules.DeleteSpaceBeforeEofToken);
            ruleMap.Add(Rules.DeleteSpaceAfterValueBeforeColon);
            ruleMap.Add(Rules.DeleteSpaceAfterValueBeforeDot);
            ruleMap.Add(Rules.DeleteSpaceBeforeValueAfterColon);
            ruleMap.Add(Rules.DeleteSpaceBeforeValueAfterDot);
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

        internal static bool NoCommentsBetweenTokens(FormattingContext formattingContext)
        {
            return !formattingContext.ContainsCommentsBetweenTokens();
        }

    }
}
