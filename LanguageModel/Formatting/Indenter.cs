/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System.Collections.Generic;
using LanguageService.Formatting.Options;
using LanguageService.Shared;
using Validation;

namespace LanguageService.Formatting
{
    internal class Indenter
    {
        internal static IEnumerable<TextEditInfo> GetIndentations(List<ParsedToken> parsedTokens, FormattingOptions formattingOptions)
        {
            Requires.NotNull(parsedTokens, nameof(parsedTokens));

            foreach (ParsedToken parsedToken in parsedTokens)
            {
                string indentationString =
                                  Indenter.GetIndentationStringFromBlockLevel(parsedToken, formattingOptions);

                foreach (IndentInfo indentInfo in Indenter.GetIndentInformation(parsedToken))
                {
                    yield return new TextEditInfo(new Range(indentInfo.Start, indentInfo.Length),
                        indentInfo.IsBeforeText ? indentationString : string.Empty);
                }
            }
        }

        private struct IndentInfo
        {
            internal IndentInfo(int start, int length, bool isBeforeText)
            {
                this.Start = start;
                this.Length = length;
                this.IsBeforeText = isBeforeText;
            }

            internal int Start { get; }

            internal int Length { get; }

            internal bool IsBeforeText { get; }
        }

        internal static int GetIndentationFromPosition(SyntaxTree syntaxTree, FormattingOptions formattingOptions, int position)
        {
            int level = GetIndentLevelFromPosition(syntaxTree, position);
            return GetTotalNumberOfSpaces(level, formattingOptions);
        }

        private static string GetIndentationStringFromBlockLevel(ParsedToken parsedToken, FormattingOptions formattingOptions)
        {
            int totalSpaces = GetTotalNumberOfSpaces(parsedToken.BlockLevel, formattingOptions);

            int spacesNeeded = totalSpaces;
            int tabsNeeded = 0;

            if (formattingOptions.UsingTabs && formattingOptions.TabSize > 0)
            {
                spacesNeeded = totalSpaces % (int)formattingOptions.TabSize;
                tabsNeeded = (totalSpaces - spacesNeeded) / (int)formattingOptions.TabSize;
            }

            return new string('\t', tabsNeeded) + new string(' ', spacesNeeded);
        }

        private static int GetTotalNumberOfSpaces(int level, FormattingOptions globalOptions)
        {
            return level * (int)globalOptions.IndentSize;
        }

        private static int GetIndentLevelFromPosition(SyntaxTree syntaxTree, int position)
        {
            SyntaxNodeOrToken currentNode = syntaxTree.Root;
            SyntaxNode parent = null;
            int blockLevel = 0;

            while (true)
            {
                if (currentNode as Token != null)
                {
                    break;
                }

                SyntaxNode syntaxNode = (SyntaxNode)currentNode;

                if ((syntaxNode.Kind == SyntaxKind.BlockNode && parent.Kind != SyntaxKind.ChunkNode) ||
                    syntaxNode.Kind == SyntaxKind.FieldList)
                {
                    blockLevel++;
                }

                if (syntaxNode.Children == null || syntaxNode.Children.Count == 0)
                {
                    break;
                }

                if (syntaxNode.Children.Count < 2)
                {
                    parent = (SyntaxNode)currentNode;
                    currentNode = syntaxNode.Children[0];
                    continue;
                }

                for (int i = 0; i < syntaxNode.Children.Count - 1; ++i)
                {
                    SyntaxNodeOrToken currentChild = syntaxNode.Children[i];
                    SyntaxNodeOrToken nextChild = syntaxNode.Children[i + 1];

                    int startCurrentChild = currentChild as SyntaxNode == null ?
                        ((Token)currentChild).FullStart :
                        ((SyntaxNode)currentChild).StartPosition;

                    int startNextChild = nextChild as SyntaxNode == null ?
                        ((Token)nextChild).Start :
                        ((SyntaxNode)nextChild).StartPosition;

                    SyntaxKind currentSyntaxKind = currentChild as SyntaxNode == null ?
                        ((Token)currentChild).Kind :
                        ((SyntaxNode)currentChild).Kind;

                    SyntaxKind nextSyntaxKind = nextChild as SyntaxNode == null ?
                        ((Token)nextChild).Kind :
                        ((SyntaxNode)nextChild).Kind;

                    bool nextTokenIsMissing = nextChild as Token != null ?
                        ((Token)nextChild).Kind == SyntaxKind.MissingToken :
                        false;

                    if ((position > startCurrentChild && position < startNextChild) ||
                        ((currentSyntaxKind == SyntaxKind.BlockNode || currentSyntaxKind == SyntaxKind.FieldList)
                        && nextTokenIsMissing) ||
                        (position == startNextChild && nextSyntaxKind == SyntaxKind.EndOfFile))
                    {
                        parent = (SyntaxNode)currentNode;
                        currentNode = currentChild;
                        break;
                    }

                    if (i + 1 >= syntaxNode.Children.Count - 1)
                    {
                        parent = (SyntaxNode)currentNode;
                        currentNode = nextChild;
                        break;
                    }
                }
            }

            return blockLevel;
        }

        private static IEnumerable<IndentInfo> GetIndentInformation(ParsedToken parsedToken)
        {
            Requires.NotNull(parsedToken, nameof(parsedToken));

            List<Trivia> leadingTrivia = parsedToken.Token.LeadingTrivia;

            if (leadingTrivia.Count <= 0)
            {
                yield break;
            }

            if (parsedToken.Token.FullStart == 0 && leadingTrivia[0].Type == SyntaxKind.Whitespace)
            {
                // First token on first line must have no indentation
                yield return new IndentInfo(parsedToken.Token.FullStart, leadingTrivia[0].Text.Length, isBeforeText: false);
            }

            int start = parsedToken.Token.FullStart;

            for (int i = 0; i < leadingTrivia.Count; ++i)
            {
                start += leadingTrivia[i].Text.Length;

                Trivia currentTrivia = leadingTrivia[i];
                if (currentTrivia.Type != SyntaxKind.Newline)
                {
                    continue;
                }

                bool isAtEndOfTrivia = i + 1 >= leadingTrivia.Count;
                if (isAtEndOfTrivia ||
                           (leadingTrivia[i + 1].Type != SyntaxKind.Whitespace &&
                           leadingTrivia[i + 1].Type != SyntaxKind.Newline))
                {
                    yield return new IndentInfo(start, 0, isBeforeText: true);
                }
                //else
                //{
                //    Trivia nextTrivia = leadingTrivia[i + 1];
                //    if (nextTrivia.Type == SyntaxKind.Whitespace)
                //    {
                //        bool isBeforeText = i + 1 == leadingTrivia.Count - 1 ||
                //            (i + 2 < leadingTrivia.Count &&
                //            (leadingTrivia[i + 2].Type != SyntaxKind.Newline ||
                //            leadingTrivia[i + 2].Type != SyntaxKind.Whitespace));
                //        yield return new IndentInfo(start, nextTrivia.Text.Length, isBeforeText);
                //    }
                //}
            }
        }
    }
}
