// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageService.Shared;
using Validation;

namespace LanguageService.Formatting
{
    internal class ParsedToken
    {
        internal ParsedToken(Token token, int blockLevel, SyntaxNode upperStatementNode, SyntaxNode immediateStatementNode)
        {
            Requires.NotNull(token, nameof(token));

            this.Token = token;
            this.BlockLevel = blockLevel;
            this.UpperStatementNode = upperStatementNode;
            this.ImmediateStatementNode = immediateStatementNode;
        }

        internal Token Token { get; }

        internal int BlockLevel { get; }

        internal SyntaxNode UpperStatementNode { get; }

        internal SyntaxNode ImmediateStatementNode { get; }

        private static readonly ImmutableHashSet<SyntaxKind> StatKinds = ImmutableHashSet.Create(
            SyntaxKind.Semicolon,
            SyntaxKind.AssignmentStatementNode,
            SyntaxKind.FunctionCallStatementNode,
            SyntaxKind.LabelStatementNode,
            SyntaxKind.BreakStatementNode,
            SyntaxKind.GoToStatementNode,
            SyntaxKind.DoStatementNode,
            SyntaxKind.WhileStatementNode,
            SyntaxKind.RepeatStatementNode,
            SyntaxKind.IfStatementNode,
            SyntaxKind.MultipleArgForStatementNode,
            SyntaxKind.SimpleForStatementNode,
            SyntaxKind.GlobalFunctionStatementNode,
            SyntaxKind.LocalFunctionStatementNode,
            SyntaxKind.LocalAssignmentStatementNode);

        private static IEnumerable<ParsedToken> WalkTreeRangeKeepLevelAndParent(SyntaxNodeOrToken currentRoot, int blockLevel, SyntaxNode upperStatementNode, SyntaxNode immediateStatementNode, Range range)
        {
            if (!currentRoot.IsLeafNode)
            {
                SyntaxNode syntaxNode = (SyntaxNode)currentRoot;

                SyntaxNode nextUpperStatementNode = immediateStatementNode;
                SyntaxNode nextImmediateStatementNode = syntaxNode;

                if (SyntaxKind.TableConstructorExp != syntaxNode.Kind &&
                    SyntaxKind.TableConstructorArg != syntaxNode.Kind &&
                    !StatKinds.Contains(syntaxNode.Kind))
                {
                    nextImmediateStatementNode = immediateStatementNode;
                    nextUpperStatementNode = upperStatementNode;
                }

                if (nextUpperStatementNode == null)
                {
                    nextUpperStatementNode = nextImmediateStatementNode;
                }

                if ((syntaxNode.Kind == SyntaxKind.BlockNode && nextImmediateStatementNode != null) ||
                     syntaxNode.Kind == SyntaxKind.FieldList)
                {
                    blockLevel++;
                }

                foreach (SyntaxNodeOrToken node in syntaxNode.Children)
                {
                    foreach (ParsedToken parsedToken in WalkTreeRangeKeepLevelAndParent(node,
                        blockLevel,
                        nextUpperStatementNode, nextImmediateStatementNode,
                        range))
                    {
                        yield return parsedToken;
                    }
                }
            }
            else
            {
                Token token = currentRoot as Token;

                if (token != null && token.Kind != SyntaxKind.MissingToken && token.Start >= range.Start)
                {
                    if (token.FullStart > range.End)
                    {
                        yield break;
                    }

                    yield return new ParsedToken(token, blockLevel, upperStatementNode, immediateStatementNode);
                }
            }
        }

        internal static IEnumerable<ParsedToken> GetParsedTokens(SyntaxTree syntaxTree, Range range)
        {
            return WalkTreeRangeKeepLevelAndParent(syntaxTree.Root, 0, null, null, range);
        }
    }
}
