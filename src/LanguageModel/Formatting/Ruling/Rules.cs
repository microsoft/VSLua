// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LanguageService.Formatting.Ruling
{
    internal class Rules
    {
        private static readonly List<Func<FormattingContext, bool>> DefaultFilters = new List<Func<FormattingContext, bool>>
        {
            TokensAreOnSameLine,
            NoCommentsBetweenTokens
        };

        internal static readonly Rule SpaceAfterComma =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.Comma, TokenRange.AnyVisible),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterAssignmentOperatorInStatement =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.AssignmentOperator, TokenRange.AnyVisible),
                new List<Func<FormattingContext, bool>>
                {
                    TokensAreOnSameLine,
                    NoCommentsBetweenTokens,
                    InSyntaxNode(Side.Left, new List<SyntaxKind>
                    {
                        SyntaxKind.AssignmentStatementNode,
                        SyntaxKind.LocalAssignmentStatementNode
                    })
                },
                RuleAction.Space);

        internal static readonly Rule SpaceBeforeAssignmentOperatorInStatement =
            new SimpleRule(
                new RuleDescriptor(TokenRange.AnyVisible, SyntaxKind.AssignmentOperator),
                new List<Func<FormattingContext, bool>>
                {
                    TokensAreOnSameLine,
                    NoCommentsBetweenTokens,
                    InSyntaxNode(Side.Right, new List<SyntaxKind>
                    {
                        SyntaxKind.AssignmentStatementNode,
                        SyntaxKind.LocalAssignmentStatementNode
                    })
                },
                RuleAction.Space);

        internal static readonly Rule SpaceAfterAssignmentOperatorInField =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.AssignmentOperator, TokenRange.AnyVisible),
                new List<Func<FormattingContext, bool>>
                {
                    TokensAreOnSameLine,
                    NoCommentsBetweenTokens,
                    InTableConstructor(Side.Left)
                },
                RuleAction.Space);

        internal static readonly Rule SpaceBeforeAssignmentOperatorInField =
            new SimpleRule(
                new RuleDescriptor(TokenRange.AnyVisible, SyntaxKind.AssignmentOperator),
                new List<Func<FormattingContext, bool>>
                {
                    TokensAreOnSameLine,
                    NoCommentsBetweenTokens,
                    InTableConstructor(Side.Right)
                },
                RuleAction.Space);

        internal static readonly Rule SpaceAfterBinaryOperator =
            new SimpleRule(
                new RuleDescriptor(TokenRange.BinaryOperators, TokenRange.AnyVisible),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceBeforeBinaryOperator =
            new SimpleRule(
                new RuleDescriptor(TokenRange.AnyVisible, TokenRange.BinaryOperators),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterValueBeforeOpenParenthesis =
            new SimpleRule(
                new RuleDescriptor(TokenRange.Value, SyntaxKind.OpenParen),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceBeforeValueAfterOpenParenthesis =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.OpenParen, TokenRange.Value),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule NoSpaceBeforeOpenSquareBracket =
            new SimpleRule(
                new RuleDescriptor(TokenRange.AnyVisible, SyntaxKind.OpenBracket),
                DefaultFilters, RuleAction.Delete);

        internal static readonly Rule SpaceBeforeOpenCurlyBraces =
            new SimpleRule(
                new RuleDescriptor(TokenRange.AnyVisible, SyntaxKind.OpenCurlyBrace),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceBeforeValueAfterOpenSquareBracket =
            new SimpleRule(
                new RuleDescriptor(SyntaxKind.OpenBracket, TokenRange.Value),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceBeforeValueAfterOpenCurlyBrace =
            new SimpleRule(new RuleDescriptor(SyntaxKind.OpenCurlyBrace, TokenRange.Value),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterValueBeforeCloseParenthesis =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.CloseParen),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterValueBeforeCloseSquareBracket =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.CloseBracket),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule SpaceAfterValueBeforeCloseCurlyBrace =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.CloseCurlyBrace),
                DefaultFilters, RuleAction.Space);

        internal static readonly Rule NoSpaceBeforeComma =
            new SimpleRule(new RuleDescriptor(TokenRange.AnyVisible, SyntaxKind.Comma),
                DefaultFilters, RuleAction.Delete);

        internal static readonly DeleteSpaceBeforeEofToken DeleteSpaceBeforeEofToken = new DeleteSpaceBeforeEofToken();

        internal static readonly Rule DeleteSpaceAfterValueBeforeDot =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.Dot),
                DefaultFilters, RuleAction.Delete);

        internal static readonly Rule DeleteSpaceBeforeValueAfterDot =
            new SimpleRule(new RuleDescriptor(SyntaxKind.Dot, TokenRange.Value),
                DefaultFilters, RuleAction.Delete);

        internal static readonly Rule DeleteSpaceAfterValueBeforeColon =
            new SimpleRule(new RuleDescriptor(TokenRange.Value, SyntaxKind.Colon),
                DefaultFilters, RuleAction.Delete);

        internal static readonly Rule DeleteSpaceBeforeValueAfterColon =
            new SimpleRule(new RuleDescriptor(SyntaxKind.Colon, TokenRange.Value),
                DefaultFilters, RuleAction.Delete);

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

        internal static readonly Rule SpaceBeforeAssignmentOperatorInFor =
            new SimpleRule(new RuleDescriptor(TokenRange.AnyVisible, SyntaxKind.AssignmentOperator),
                new List<Func<FormattingContext, bool>>
                {
                    TokensAreOnSameLine,
                    NoCommentsBetweenTokens,
                    InSyntaxNode(Side.Right, new List<SyntaxKind> { SyntaxKind.SimpleForStatementNode })
                },
                RuleAction.Space);

        internal static readonly Rule SpaceAfterAssignmentOperatorInFor =
            new SimpleRule(new RuleDescriptor(SyntaxKind.AssignmentOperator, TokenRange.AnyVisible),
                new List<Func<FormattingContext, bool>>
                {
                    TokensAreOnSameLine,
                    NoCommentsBetweenTokens,
                    InSyntaxNode(Side.Left, new List<SyntaxKind> { SyntaxKind.SimpleForStatementNode })
                },
                RuleAction.Space);

        internal static readonly ImmutableArray<Rule> AllRules = ImmutableArray.Create(
            NoSpaceAfterCommaInFor,
            SpaceAfterComma,
            NoSpaceBeforeComma,
            SpaceAfterAssignmentOperatorInStatement,
            SpaceBeforeAssignmentOperatorInStatement,
            SpaceBeforeAssignmentOperatorInFor,
            SpaceAfterAssignmentOperatorInFor,
            SpaceBeforeAssignmentOperatorInField,
            SpaceAfterAssignmentOperatorInField,
            SpaceAfterBinaryOperator,
            SpaceBeforeBinaryOperator,
            SpaceBeforeOpenCurlyBraces,
            NoSpaceBeforeOpenSquareBracket,
            SpaceAfterValueBeforeOpenParenthesis,
            SpaceBeforeValueAfterOpenParenthesis,
            SpaceBeforeValueAfterOpenSquareBracket,
            SpaceBeforeValueAfterOpenCurlyBrace,
            SpaceAfterValueBeforeCloseParenthesis,
            SpaceAfterValueBeforeCloseSquareBracket,
            SpaceAfterValueBeforeCloseCurlyBrace,
            DeleteSpaceAfterValueBeforeDot,
            DeleteSpaceBeforeValueAfterDot,
            DeleteSpaceAfterValueBeforeColon,
            DeleteSpaceBeforeValueAfterColon,
            DeleteSpaceBeforeEofToken,
            DeleteTrailingWhitespace);

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

        /// <summary>
        /// Describes the side the token in question
        /// </summary>
        private enum Side
        {
            /// <summary>
            /// The left token
            /// </summary>
            Left,

            /// <summary>
            /// The right token
            /// </summary>
            Right
        }

        private static ParsedToken GetTokenOn(Side side, FormattingContext formattingContext)
        {
            return (side == Side.Left) ?
                    formattingContext.CurrentToken :
                    formattingContext.NextToken;
        }

        private static Func<FormattingContext, bool> InTableConstructor(Side side)
        {
            return (FormattingContext formattingContext) =>
            {
                ParsedToken parsedToken = GetTokenOn(side, formattingContext);

                bool upperStatementNodeIsTableConstructor =
                    parsedToken.UpperStatementNode != null &&
                    (SyntaxKind.TableConstructorArg == parsedToken.ImmediateStatementNode.Kind ||
                    SyntaxKind.TableConstructorExp == parsedToken.ImmediateStatementNode.Kind);

                return upperStatementNodeIsTableConstructor;
            };
        }

        private static Func<FormattingContext, bool> InSyntaxNode(Side side, List<SyntaxKind> statementKinds)
        {
            return (FormattingContext formattingContext) =>
            {
                ParsedToken parsedToken = GetTokenOn(side, formattingContext);

                bool containsImmediateStatementNode =
                parsedToken.ImmediateStatementNode != null &&
                statementKinds.Contains(parsedToken.ImmediateStatementNode.Kind);

                return containsImmediateStatementNode;
            };
        }

        private static Func<FormattingContext, bool> IsStartOfStatement(Side side)
        {
            return (FormattingContext formattingContext) =>
            {
                ParsedToken parsedToken = GetTokenOn(side, formattingContext);

                return parsedToken.Token.FullStart == parsedToken.ImmediateStatementNode.StartPosition;
            };
        }

        private static Func<FormattingContext, bool> InForLoopOnSameLine(Side side)
        {
            return (FormattingContext formattingContext) =>
            {
                ParsedToken parsedToken = GetTokenOn(side, formattingContext);
                SourceText sourceText = formattingContext.SourceText;

                int start;
                int end;

                if (parsedToken.UpperStatementNode.Kind == SyntaxKind.SimpleForStatementNode)
                {
                    SimpleForStatementNode forStatement = (SimpleForStatementNode)parsedToken.UpperStatementNode;
                    start = forStatement.ForKeyword.Start;
                    end = forStatement.EndKeyword.Start;

                    return sourceText.GetLineNumberFromIndex(start) == sourceText.GetLineNumberFromIndex(end);
                }
                else if (parsedToken.UpperStatementNode.Kind == SyntaxKind.MultipleArgForStatementNode)
                {
                    MultipleArgForStatementNode forStatement = (MultipleArgForStatementNode)parsedToken.UpperStatementNode;
                    start = forStatement.ForKeyword.Start;
                    end = forStatement.EndKeyword.Start;

                    return sourceText.GetLineNumberFromIndex(start) == sourceText.GetLineNumberFromIndex(end);
                }

                return false;
            };
        }

        private static Func<FormattingContext, bool> InFunctionStatementOnOneLine(Side side)
        {
            return (FormattingContext formattingContext) =>
            {
                ParsedToken parsedToken = GetTokenOn(side, formattingContext);
                SourceText sourceText = formattingContext.SourceText;

                int start;
                int end;

                if (parsedToken.UpperStatementNode.Kind == SyntaxKind.GlobalFunctionStatementNode)
                {
                    GlobalFunctionStatementNode function = (GlobalFunctionStatementNode)parsedToken.UpperStatementNode;
                    start = function.FunctionKeyword.Start;
                    end = function.FuncBody.EndKeyword.Start;

                    return sourceText.GetLineNumberFromIndex(start) == sourceText.GetLineNumberFromIndex(end);
                }
                else if (parsedToken.UpperStatementNode.Kind == SyntaxKind.LocalFunctionStatementNode)
                {
                    LocalFunctionStatementNode function = (LocalFunctionStatementNode)parsedToken.UpperStatementNode;
                    start = function.LocalKeyword.Start;
                    end = function.FuncBody.EndKeyword.Start;

                    return sourceText.GetLineNumberFromIndex(start) == sourceText.GetLineNumberFromIndex(end);
                }

                return false;
            };
        }
    }
}
