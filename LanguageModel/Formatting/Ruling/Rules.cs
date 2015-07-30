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

        internal static Rule SpaceAfterComma =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.Comma, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterAssignmentOperator =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.AssignmentOperator, TokenRange.AnyVisible),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeAssignmentOperator =
            new SimpleRule(
                new RuleDescriptor(TokenRange.AnyVisible, SyntaxKind.AssignmentOperator),
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
                new RuleDescriptor(TokenRange.Value, SyntaxKind.OpenParen),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeValueAfterOpenParenthesis =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.OpenParen, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeValueAfterOpenSquareBracket =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.OpenBracket, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceBeforeValueAfterOpenCurlyBrace =
            new SimpleRule(new RuleDescriptor(SyntaxKind.OpenCurlyBrace, TokenRange.Value),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeCloseParenthesis =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.CloseParen),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeCloseSquareBracket =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.CloseBracket),
                defaultFilters, RuleAction.Space);

        internal static Rule SpaceAfterValueBeforeCloseCurlyBrace =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.CloseCurlyBrace),
                defaultFilters, RuleAction.Space);

        internal static Rule DeleteSpaceBeforeEofToken =
            new SimpleRule(new RuleDescriptor(TokenRange.AnyVisible, SyntaxKind.EndOfFile),
                defaultFilters, RuleAction.Delete);

        internal static Rule DeleteTrailingWhitespace = new DeleteTrailingWhitespaceRule();

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
