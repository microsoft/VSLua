using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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

        internal static ImmutableArray<Rule> AllRules = ImmutableArray.Create(
            SpaceAfterComma,
            SpaceAfterAssignmentOperator,
            SpaceBeforeAssignmentOperator,
            SpaceAfterBinaryOperator,
            SpaceBeforeBinaryOperator,
            SpaceAfterValueBeforeOpenParenthesis,
            SpaceBeforeValueAfterOpenParenthesis,
            SpaceBeforeValueAfterOpenSquareBracket,
            SpaceBeforeValueAfterOpenCurlyBrace,
            SpaceAfterValueBeforeCloseParenthesis,
            SpaceAfterValueBeforeCloseSquareBracket,
            SpaceAfterValueBeforeCloseCurlyBrace,
            DeleteSpaceBeforeEofToken,
            DeleteSpaceAfterValueBeforeDot,
            DeleteSpaceBeforeValueAfterDot,
            DeleteSpaceAfterValueBeforeColon,
            DeleteSpaceBeforeValueAfterColon,
            DeleteTrailingWhitespace
            );

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
