using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageService.Shared;
using Validation;

namespace LanguageService.Formatting
{
    internal class ParsedToken
    {
        internal ParsedToken(Token token, int blockLevel, SyntaxNode statementNode)
        {
            Requires.NotNull(token, nameof(token));

            this.Token = token;
            this.BlockLevel = blockLevel;
            this.StatementNode = statementNode;
        }

        internal Token Token { get; }
        internal int BlockLevel { get; }
        internal SyntaxNode StatementNode { get; }


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
            SyntaxKind.LocalAssignmentStatementNode
        );

        private static IEnumerable<ParsedToken> WalkTreeRangeKeepLevelAndParent(SyntaxNodeOrToken currentRoot, int blockLevel, SyntaxNode statementNode, Range range)
        {
            if (!SyntaxTree.IsLeafNode(currentRoot))
            {
                SyntaxNode syntaxNode = (SyntaxNode)currentRoot;
                foreach (SyntaxNodeOrToken node in syntaxNode.Children)
                {
                    SyntaxNode nextStatementNode = syntaxNode;
                    bool increaseBlockLevel = false;

                    if (!StatKinds.Contains(syntaxNode.Kind))
                    {
                        nextStatementNode = statementNode;
                    }

                    if ((syntaxNode.Kind == SyntaxKind.BlockNode && nextStatementNode != null) ||
                         syntaxNode.Kind == SyntaxKind.FieldList)
                    {
                        increaseBlockLevel = true;
                    }

                    foreach (ParsedToken parsedToken in WalkTreeRangeKeepLevelAndParent(node,
                        increaseBlockLevel ? blockLevel + 1 : blockLevel,
                        nextStatementNode, range))
                    {
                        yield return parsedToken;
                    }
                }
            }
            else
            {
                Token token = currentRoot as Token;

                if (token != null && token.Start >= range.Start)
                {
                    if (token.FullStart > range.End)
                    {
                        yield break;
                    }
                    yield return new ParsedToken(token, blockLevel, statementNode);
                }
            }
        }

        internal static IEnumerable<ParsedToken> GetParsedTokens(SyntaxTree syntaxTree, Range range)
        {
            return WalkTreeRangeKeepLevelAndParent(syntaxTree.Root, 0, null, range);
        }
    }
}
