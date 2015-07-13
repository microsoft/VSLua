using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting.Ruling
{
    internal class Rules
    {

        private static readonly List<ContextFilter> defaultFilters = new List<ContextFilter>
        {
            TokensAreOnSameLine,
            NoCommentBetweenTokens
        };

        internal static Rule SpaceAfterComma =
            new Rule(
                new RuleDescriptor(TokenType.Comma, TokenRange.Any),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterAssignmentOperator =
            new Rule(
                new RuleDescriptor(TokenRange.Any, TokenType.AssignmentOperator),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeAssignmentOperator =
            new Rule(
                new RuleDescriptor(TokenRange.Any, TokenType.AssignmentOperator),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterBinaryOperator =
            new Rule(
                new RuleDescriptor(TokenRange.BinaryOperators, TokenRange.Any),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeBinaryOperator =
            new Rule(
                new RuleDescriptor(TokenRange.Any, TokenRange.BinaryOperators),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeOpenParenthesis =
            new Rule(
                new RuleDescriptor(TokenRange.Value, TokenType.OpenParen),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeValueAfterOpenParenthesis =
            new Rule(
                new RuleDescriptor(TokenType.OpenParen, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeValueAfterOpenSquareBracket =
            new Rule(
                new RuleDescriptor(TokenType.OpenBracket, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeValueAfterOpenCurlyBrace =
            new Rule(new RuleDescriptor(TokenType.OpenCurlyBrace, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeCloseParenthesis =
            new Rule(new RuleDescriptor(TokenRange.Value, TokenType.CloseParen),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeCloseSquareBracket =
            new Rule(new RuleDescriptor(TokenRange.Value, TokenType.CloseBracket),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeCloseCurlyBrace =
            new Rule(new RuleDescriptor(TokenRange.Value, TokenType.CloseCurlyBrace),
                defaultFilters, RuleAction.Space);



        internal static bool TokensAreOnSameLine(FormattingContext formattingContext)
        {
            return formattingContext.TokensOnSameLine();
        }

        internal static bool NoCommentBetweenTokens(FormattingContext formattingContext)
        {
            return !formattingContext.CommentsInBetween();
        }

    }
}
