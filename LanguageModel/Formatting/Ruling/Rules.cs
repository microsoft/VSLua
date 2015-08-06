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

        internal static readonly Rule SpaceAfterComma =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.Comma, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterAssignmentOperator =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.AssignmentOperator, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceBeforeAssignmentOperator =
            new SimpleRule(
                new RuleDescriptor(TokenRange.AnyVisible, SyntaxKind.AssignmentOperator),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterBinaryOperator =
            new SimpleRule(
                new RuleDescriptor(TokenRange.BinaryOperators, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceBeforeBinaryOperator =
            new SimpleRule(
                new RuleDescriptor(TokenRange.AnyVisible, TokenRange.BinaryOperators),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterValueBeforeOpenParenthesis =
            new SimpleRule(
                new RuleDescriptor(TokenRange.Value, SyntaxKind.OpenParen),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceBeforeValueAfterOpenParenthesis =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.OpenParen, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceBeforeValueAfterOpenSquareBracket =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.OpenBracket, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceBeforeValueAfterOpenCurlyBrace =
            new SimpleRule(new RuleDescriptor(SyntaxKind.OpenCurlyBrace, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterValueBeforeCloseParenthesis =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.CloseParen),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterValueBeforeCloseSquareBracket =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.CloseBracket),
                defaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterValueBeforeCloseCurlyBrace =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.CloseCurlyBrace),
                defaultFilters, RuleAction.Space);

        internal static readonly DeleteSpaceBeforeEofToken DeleteSpaceBeforeEofToken = new DeleteSpaceBeforeEofToken();

        internal static readonly Rule DeleteSpaceAfterValueBeforeDot =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.Dot),
                defaultFilters, RuleAction.Delete);

        internal static readonly Rule DeleteSpaceBeforeValueAfterDot =
            new SimpleRule(new RuleDescriptor(SyntaxKind.Dot, TokenRange.Value),
                defaultFilters, RuleAction.Delete);

        internal static readonly Rule DeleteSpaceAfterValueBeforeColon =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.Colon),
                defaultFilters, RuleAction.Delete);

        internal static readonly Rule DeleteSpaceBeforeValueAfterColon =
            new SimpleRule(new RuleDescriptor(SyntaxKind.Colon, TokenRange.Value),
                defaultFilters, RuleAction.Delete);

        internal static readonly Rule DeleteTrailingWhitespace = new DeleteTrailingWhitespace();

        internal static readonly Rule NoSpaceAfterCommaInFor =
            new SimpleRule(new RuleDescriptor(SyntaxKind.Comma, TokenRange.AnyVisible),
                new List<Func<FormattingContext, bool>>
                {
                    TokensAreOnSameLine,
                    NoCommentsBetweenTokens,
                    InSyntaxNode(Side.Left, new List<SyntaxKind> { SyntaxKind.SimpleForStatementNode })
                        },

                RuleAction.Delete);



        internal static readonly ImmutableArray<Rule> AllRules = ImmutableArray.Create(
            NoSpaceAfterCommaInFor,
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

        private enum Side
        {
            Left,
            Right
        }

        private static Func<FormattingContext, bool> InSyntaxNode(Side side, List<SyntaxKind> statementKinds)
        {
            return (FormattingContext formattingContext) =>
            {
                ParsedToken parsedToken = (side == Side.Left) ?
                    formattingContext.CurrentToken :
                    formattingContext.NextToken;

                return statementKinds.Contains(parsedToken.StatementNode.Kind);
            };
        }

    }
}
