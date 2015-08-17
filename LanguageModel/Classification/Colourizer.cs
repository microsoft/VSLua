/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LanguageService.Shared;
using Validation;

namespace LanguageService.Classification
{
    public class Colourizer
    {
        internal Colourizer(ParseTreeCache parseTreeCache)
        {
            this.ParseTreeCache = parseTreeCache;
        }

        private ParseTreeCache ParseTreeCache { get; }

        public IEnumerable<TagInfo> ColorizeLexerTokens(SourceText sourceText, List<Range> ranges)
        {
            foreach (Token token in Lexer.Tokenize(sourceText.TextReader))
            {
                foreach (TagInfo tagInfo in GetTokenTagInfoFromLexer(ranges, token))
                {
                    yield return tagInfo;
                }
            }
        }

        public IEnumerable<TagInfo> ColorizeParserTokens(SourceText sourceText)
        {
            SyntaxTree syntaxTree = this.ParseTreeCache.Get(sourceText);
            return GetTokenTagInfoFromParser(syntaxTree.Root, new HashSet<string>(), new HashSet<string>(), new Token[1]);
        }

        private static IEnumerable<TagInfo> GetTokenTagInfoFromParser(
            SyntaxNodeOrToken currentRoot,
            HashSet<string> locals,
            HashSet<string> paramrefs,
            Token[] previousToken)
        {
            if (!SyntaxTree.IsLeafNode(currentRoot))
            {
                SyntaxKind syntaxKindCurrentRoot =
                        (currentRoot as Token == null) ?
                        ((SyntaxNode)currentRoot).Kind : ((Token)currentRoot).Kind;

                SyntaxNode syntaxNode = (SyntaxNode)currentRoot;
                foreach (SyntaxNodeOrToken syntaxNodeOrToken in ((SyntaxNode)currentRoot).Children)
                {
                    SyntaxKind syntaxKindChild =
                            syntaxNodeOrToken as Token == null ?
                            ((SyntaxNode)syntaxNodeOrToken).Kind : ((Token)syntaxNodeOrToken).Kind;

                    if (syntaxKindChild == SyntaxKind.LocalAssignmentStatementNode ||
                        syntaxKindChild == SyntaxKind.LocalFunctionStatementNode)
                    {
                        locals = new HashSet<string>(locals);
                        foreach (string identifier in GetLocalIdentifiers(syntaxNodeOrToken))
                        {
                            if (!locals.Contains(identifier))
                            {
                                locals.Add(identifier);
                                if (paramrefs.Contains(identifier))
                                {
                                    // if a local is defined in a block, the parameter references
                                    //   are not used, so we need to remove its precedence.
                                    paramrefs.Remove(identifier);
                                }
                            }
                        }
                    }
                    else if (syntaxKindChild == SyntaxKind.FuncBodyNode)
                    {
                        paramrefs = new HashSet<string>(paramrefs);
                        foreach (string identifier in GetParamrefIdentifiers((FuncBodyNode)syntaxNodeOrToken))
                        {
                            if (!paramrefs.Contains(identifier))
                            {
                                paramrefs.Add(identifier);
                            }
                        }
                    }

                    if (syntaxKindChild == SyntaxKind.FieldList)
                    {
                        // pass for now
                    }

                    foreach (TagInfo tagInfo in GetTokenTagInfoFromParser(syntaxNodeOrToken,
                        locals,
                        paramrefs,
                        previousToken))
                    {
                        yield return tagInfo;
                    }
                }
            }
            else
            {
                Token token = currentRoot as Token;

                if (token != null && token.Kind == SyntaxKind.Identifier)
                {
                    Classification classification;

                    if (previousToken[0] != null && (previousToken[0].Kind == SyntaxKind.Dot || previousToken[0].Kind == SyntaxKind.Colon))
                    {
                        // Everything after a dot or colon counts as a field for now...
                        //   The reason this is here instead of the lexer is for consistency,
                        //   so all "Field" classifications update at the same time, this is just convenient.
                        classification = Classification.Field;
                    }
                    else if (paramrefs.Contains(token.Text))
                    {
                        classification = Classification.ParameterReference;
                    }
                    else if (locals.Contains(token.Text))
                    {
                        classification = Classification.Local;
                    }
                    else
                    {
                        classification = Classification.Global;
                    }

                    yield return new TagInfo(token.Start, token.Length, classification);
                }

                // I use an array so I can keep track of the previous token and avoid having
                //   a pointer to pointer reference, which is kind of a pain.
                previousToken[0] = token == null ? null : token;
            }
        }

        private static List<string> GetParamrefIdentifiers(FuncBodyNode funcBody)
        {
            ParList paramList = funcBody.ParameterList;

            List<string> names = new List<string>();

            if (paramList.Children.Count > 0 && paramList.Children[0] as SeparatedList != null)
            {
                var nameList = (SeparatedList)paramList.Children[0];

                names.AddRange(GetNamesFromNameList(nameList));
            }

            return names;
        }

        private static List<string> GetLocalIdentifiers(SyntaxNodeOrToken localAssignmentOrFunction)
        {
            List<string> names = new List<string>();

            if (localAssignmentOrFunction as LocalFunctionStatementNode != null)
            {
                names.Add(((LocalFunctionStatementNode)localAssignmentOrFunction).Name.Text);
                return names;
            }

            LocalAssignmentStatementNode localAssignment = (LocalAssignmentStatementNode)localAssignmentOrFunction;

            names.AddRange(GetNamesFromNameList(localAssignment.NameList));

            return names;
        }

        private static IEnumerable<string> GetNamesFromNameList(SeparatedList nameList)
        {
            foreach (SeparatedListElement listElement in nameList.SyntaxList)
            {
                if (listElement.Element as Token != null)
                {
                    yield return ((Token)listElement.Element).Text;
                }
            }
        }

        private static IEnumerable<TagInfo> GetTokenTagInfoFromLexer(List<Range> ranges, Token token)
        {
            foreach (Range range in ranges)
            {
                int start = token.FullStart;
                foreach (Trivia trivia in token.LeadingTrivia)
                {
                    int triviaLength = trivia.Text.Length;
                    if (trivia.Type != SyntaxKind.Newline && trivia.Type != SyntaxKind.Whitespace)
                    {
                        if (trivia.Type != SyntaxKind.Comment)
                        {
                            int triviaEnd = start + triviaLength;
                            int tagStart = start > range.Start ? start : range.Start;
                            int tagEnd = triviaEnd < range.End ? triviaEnd : range.End;
                            if (tagStart < tagEnd)
                            {
                                yield return new TagInfo(tagStart, tagEnd - tagStart, SyntaxKindClassifications[trivia.Type]);
                            }
                        }
                        else
                        {
                            yield return new TagInfo(start, triviaLength, SyntaxKindClassifications[SyntaxKind.Comment]);
                        }
                    }

                    start += triviaLength;
                }

                if (token.Kind != SyntaxKind.EndOfFile && token.Kind != SyntaxKind.Identifier)
                {
                    int tokenStart = token.Start > range.Start ? start : range.Start;
                    int tokenEnd = token.End < range.End ? token.End : range.End;
                    if (tokenStart < tokenEnd)
                    {
                        yield return new TagInfo(tokenStart, tokenEnd - tokenStart, SyntaxKindClassifications[token.Kind]);
                    }
                }
            }
        }

        private static IEnumerable<Token> GetTokens(SyntaxNodeOrToken currentRoot)
        {
            if (!SyntaxTree.IsLeafNode(currentRoot))
            {
                SyntaxNode syntaxNode = (SyntaxNode)currentRoot;
                foreach (SyntaxNodeOrToken node in syntaxNode.Children)
                {
                    foreach (Token token in GetTokens(node))
                    {
                        yield return token;
                    }
                }
            }
            else
            {
                Token token = currentRoot as Token;

                if (token != null)
                {
                    yield return token;
                }
            }
        }

        private static readonly ImmutableDictionary<SyntaxKind, Classification> SyntaxKindClassifications =
            new Dictionary<SyntaxKind, Classification>()
            {
                { SyntaxKind.AndBinop, Classification.Punctuation },
                { SyntaxKind.AssignmentOperator, Classification.Operator },
                { SyntaxKind.BitwiseAndOperator, Classification.Operator },
                { SyntaxKind.BitwiseLeftOperator, Classification.Operator },
                { SyntaxKind.BitwiseOrOperator, Classification.Operator },
                { SyntaxKind.BitwiseRightOperator, Classification.Operator },
                { SyntaxKind.BreakKeyword, Classification.Keyword },
                { SyntaxKind.CloseBracket, Classification.Bracket },
                { SyntaxKind.CloseCurlyBrace, Classification.Bracket },
                { SyntaxKind.CloseParen, Classification.Bracket },
                { SyntaxKind.Colon, Classification.Punctuation },
                { SyntaxKind.Comma, Classification.Punctuation },
                { SyntaxKind.Comment, Classification.Comment },
                { SyntaxKind.DivideOperator, Classification.Punctuation },
                { SyntaxKind.DoKeyword, Classification.Keyword },
                { SyntaxKind.Dot, Classification.Punctuation },
                { SyntaxKind.DoubleColon, Classification.Punctuation },
                { SyntaxKind.ElseIfKeyword, Classification.Keyword },
                { SyntaxKind.ElseKeyword, Classification.Keyword },
                { SyntaxKind.EndKeyword, Classification.Keyword },
                { SyntaxKind.EqualityOperator, Classification.Operator },
                { SyntaxKind.ExponentOperator, Classification.Operator },
                { SyntaxKind.FalseKeyValue, Classification.KeyValue },
                { SyntaxKind.FloorDivideOperator, Classification.Operator },
                { SyntaxKind.ForKeyword, Classification.Keyword },
                { SyntaxKind.FunctionKeyword, Classification.Keyword },
                { SyntaxKind.GotoKeyword, Classification.Keyword },
                { SyntaxKind.GreaterOrEqualOperator, Classification.Operator },
                { SyntaxKind.GreaterThanOperator, Classification.Operator },
                { SyntaxKind.IfKeyword, Classification.Keyword },
                { SyntaxKind.InKeyword, Classification.Keyword },
                { SyntaxKind.LengthUnop, Classification.Punctuation },
                { SyntaxKind.LessOrEqualOperator, Classification.Operator },
                { SyntaxKind.LessThanOperator, Classification.Operator },
                { SyntaxKind.LocalKeyword, Classification.Keyword },
                { SyntaxKind.MinusOperator, Classification.Punctuation },
                { SyntaxKind.ModulusOperator, Classification.Operator },
                { SyntaxKind.MultiplyOperator, Classification.Operator },
                { SyntaxKind.NilKeyValue, Classification.KeyValue },
                { SyntaxKind.NotEqualsOperator, Classification.Operator },
                { SyntaxKind.NotUnop, Classification.Punctuation },
                { SyntaxKind.Number, Classification.Number },
                { SyntaxKind.OpenBracket, Classification.Bracket },
                { SyntaxKind.OpenCurlyBrace, Classification.Bracket },
                { SyntaxKind.OpenParen, Classification.Bracket },
                { SyntaxKind.OrBinop, Classification.Punctuation },
                { SyntaxKind.PlusOperator, Classification.Operator },
                { SyntaxKind.RepeatKeyword, Classification.Keyword },
                { SyntaxKind.ReturnKeyword, Classification.Keyword },
                { SyntaxKind.Semicolon, Classification.Punctuation },
                { SyntaxKind.String, Classification.StringLiteral },
                { SyntaxKind.StringConcatOperator, Classification.Operator },
                { SyntaxKind.ThenKeyword, Classification.Keyword },
                { SyntaxKind.TildeUnOp, Classification.Punctuation },
                { SyntaxKind.TrueKeyValue, Classification.KeyValue },
                { SyntaxKind.UnterminatedString, Classification.StringLiteral },
                { SyntaxKind.UntilKeyword, Classification.Keyword },
                { SyntaxKind.WhileKeyword, Classification.Keyword },
                { SyntaxKind.Unknown, Classification.Global }
            }

            .ToImmutableDictionary();
    }
}
