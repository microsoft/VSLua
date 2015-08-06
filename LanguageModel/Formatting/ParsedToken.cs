using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageService.Shared;
using Validation;

namespace LanguageService.Formatting
{
    internal class ParsedToken
    {
        internal ParsedToken(Token token, int blockLevel, SyntaxNode statementNode, SyntaxNode functionDefOrTableConstructor)
        {
            Requires.NotNull(token, nameof(token));

            this.Token = token;
            this.BlockLevel = blockLevel;
            this.StatementNode = statementNode;
            this.FunctionDefOrTableConstructor = functionDefOrTableConstructor;
        }

        internal Token Token { get; }
        internal int BlockLevel { get; }
        internal SyntaxNode StatementNode { get; }
        internal SyntaxNode FunctionDefOrTableConstructor { get;}


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

        private static IEnumerable<ParsedToken> WalkTreeRangeKeepLevelAndParent(SyntaxNodeOrToken currentRoot, int blockLevel, SyntaxNode statementNode, SyntaxNode closeParent = null)
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

                    if (syntaxNode.Kind == SyntaxKind.FunctionDef ||
                        syntaxNode.Kind == SyntaxKind.TableConstructorArg ||
                        syntaxNode.Kind == SyntaxKind.TableConstructorExp)
                    {
                        closeParent = syntaxNode;
                    }

                    if ((syntaxNode.Kind == SyntaxKind.BlockNode && nextStatementNode != null) ||
                         syntaxNode.Kind == SyntaxKind.FieldList)
                    {
                        increaseBlockLevel = true;
                    }

                    foreach (ParsedToken parsedToken in WalkTreeRangeKeepLevelAndParent(node,
                        increaseBlockLevel ? blockLevel + 1 : blockLevel,
                        nextStatementNode, closeParent))
                    {
                        yield return parsedToken;
                    }
                }
            }
            else
            {
                Token token = currentRoot as Token;
                if (token != null)
                {
                    yield return new ParsedToken(token, blockLevel, statementNode, closeParent);
                }
            }
        }

        internal static IEnumerable<ParsedToken> GetParsedTokens(SyntaxTree syntaxTree, Range range)
        {
            return WalkTreeRangeKeepLevelAndParent(syntaxTree.Root, 0, null);
        }

        //// This function wouldn't exist in the final version since instead of iterating
        ////   through all the tokens from the lexer, I'd just walk the parsetree from the start
        //internal static List<ParsedToken> GetParsedTokens(List<Token> tokens, int from, int to)
        //{
        //    Requires.NotNull(tokens, nameof(tokens));

        //    List<ParsedToken> parsedTokens = new List<ParsedToken>();

        //    int indent_level = 0;
        //    foreach (Token token in tokens)
        //    {


        //        if (token.FullStart > to)
        //    {
        //            break;
        //        }


        //        if (DecreaseIndentOn.Contains(token.Kind))
        //        {
        //            indent_level--;
        //        }

        //        indent_level = indent_level < 0 ? 0 : indent_level;
        //        if (token.Start >= from)
        //        {
        //        parsedTokens.Add(new ParsedToken(token, indent_level, null));
        //        }
        //        if (IncreaseIndentAfter.Contains(token.Kind))
        //        {
        //            indent_level++;
        //        }

        //        if (token.Kind == SyntaxKind.ReturnKeyword)
        //        {
        //            indent_level--;
        //        }
        //    }

        //    return parsedTokens;
        //}

        private static readonly ImmutableArray<SyntaxKind> IncreaseIndentAfter = ImmutableArray.Create
            (
        SyntaxKind.DoKeyword,
        SyntaxKind.ThenKeyword,
        SyntaxKind.ElseKeyword,
        SyntaxKind.FunctionKeyword,
        SyntaxKind.OpenCurlyBrace
            );

        private static readonly ImmutableArray<SyntaxKind> DecreaseIndentOn = ImmutableArray.Create
            (
                SyntaxKind.EndKeyword,
                SyntaxKind.ElseIfKeyword,
                SyntaxKind.CloseCurlyBrace,
                SyntaxKind.ElseKeyword
            );
    }
}
