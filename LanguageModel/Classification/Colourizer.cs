using System.Collections.Generic;
using System.Collections.Immutable;

namespace LanguageService.Classification
{
    public class Colourizer
    {
        internal Colourizer(ParseTreeCache parseTreeCache)
        {
            this.ParseTreeCache = parseTreeCache;
        }

        private ParseTreeCache ParseTreeCache { get; }

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

        public IEnumerable<TagInfo> Colourize(SourceText sourceText)
        {
            SyntaxTree syntaxTree = this.ParseTreeCache.Get(sourceText);

            foreach (Token token in GetTokens(syntaxTree.Root))
            {
                int start = token.FullStart;
                foreach (Trivia trivia in token.LeadingTrivia)
                {
                    int length = trivia.Text.Length;
                    Classification triviaClass = Classification.Comment;
                    if (trivia.Type != SyntaxKind.Whitespace || trivia.Type != SyntaxKind.Newline)
                    {
                        SyntaxKindClassifications.TryGetValue(trivia.Type, out triviaClass);
                        yield return new TagInfo(start, length, triviaClass);
                    }
                    start += trivia.Text.Length;
                }

                Classification classification = Classification.Keyword;
                SyntaxKindClassifications.TryGetValue(token.Kind, out classification);

                if (token.Kind != SyntaxKind.EndOfFile && token.Start >= 0 && token.Length >= 0)
                {
                    yield return new TagInfo(token.Start, token.Length, classification);
                }
            }
        }

        private static readonly ImmutableDictionary<SyntaxKind, Classification> SyntaxKindClassifications =
            new Dictionary<SyntaxKind, Classification>()
            {
                {SyntaxKind.AndBinop, Classification.Punctuation},
                {SyntaxKind.AssignmentOperator, Classification.Operator },
                {SyntaxKind.BitwiseAndOperator, Classification.Operator},
                {SyntaxKind.BitwiseLeftOperator, Classification.Operator},
                {SyntaxKind.BitwiseOrOperator, Classification.Operator},
                {SyntaxKind.BitwiseRightOperator, Classification.Operator},
                {SyntaxKind.BreakKeyword, Classification.Keyword},
                {SyntaxKind.CloseBracket, Classification.Punctuation},
                {SyntaxKind.CloseCurlyBrace, Classification.Punctuation},
                {SyntaxKind.CloseParen, Classification.Punctuation},
                {SyntaxKind.Colon, Classification.Punctuation},
                {SyntaxKind.Comma, Classification.Punctuation},
                {SyntaxKind.Comment, Classification.Comment},
                {SyntaxKind.DivideOperator, Classification.Punctuation},
                {SyntaxKind.DoKeyword, Classification.Keyword},
                {SyntaxKind.Dot, Classification.Punctuation},
                {SyntaxKind.DoubleColon, Classification.Punctuation},
                {SyntaxKind.ElseIfKeyword, Classification.Keyword},
                {SyntaxKind.ElseKeyword, Classification.Keyword},
                {SyntaxKind.EndKeyword, Classification.Keyword},
                {SyntaxKind.EqualityOperator, Classification.Operator},
                {SyntaxKind.ExponentOperator, Classification.Operator},
                { SyntaxKind.FalseKeyValue, Classification.KeyValue },
                {SyntaxKind.FloorDivideOperator, Classification.Operator},
                {SyntaxKind.ForKeyword, Classification.Keyword},
                {SyntaxKind.FunctionKeyword, Classification.Keyword},
                {SyntaxKind.GotoKeyword, Classification.Keyword},
                {SyntaxKind.GreaterOrEqualOperator, Classification.Operator},
                {SyntaxKind.GreaterThanOperator, Classification.Operator},
                {SyntaxKind.Identifier, Classification.Identifier},
                {SyntaxKind.IfKeyword, Classification.Keyword},
                {SyntaxKind.InKeyword, Classification.Keyword},
                {SyntaxKind.LengthUnop, Classification.Punctuation},
                {SyntaxKind.LessOrEqualOperator, Classification.Operator},
                {SyntaxKind.LessThanOperator, Classification.Operator},
                {SyntaxKind.LocalKeyword, Classification.Keyword},
                {SyntaxKind.MinusOperator, Classification.Punctuation},
                {SyntaxKind.ModulusOperator, Classification.Operator},
                {SyntaxKind.MultiplyOperator, Classification.Operator},
                {SyntaxKind.NilKeyValue, Classification.KeyValue},
                {SyntaxKind.NotEqualsOperator, Classification.Operator},
                {SyntaxKind.NotUnop, Classification.Punctuation},
                {SyntaxKind.Number, Classification.Number},
                {SyntaxKind.OpenBracket, Classification.Punctuation},
                {SyntaxKind.OpenCurlyBrace, Classification.Punctuation},
                {SyntaxKind.OpenParen, Classification.Punctuation},
                {SyntaxKind.OrBinop, Classification.Punctuation},
                {SyntaxKind.PlusOperator, Classification.Operator},
                {SyntaxKind.RepeatKeyword, Classification.Keyword},
                {SyntaxKind.ReturnKeyword, Classification.Keyword},
                {SyntaxKind.Semicolon, Classification.Punctuation},
                {SyntaxKind.String, Classification.StringLiteral},
                {SyntaxKind.StringConcatOperator, Classification.Operator},
                {SyntaxKind.ThenKeyword, Classification.Keyword},
                {SyntaxKind.TildeUnOp, Classification.Punctuation},
                {SyntaxKind.TrueKeyValue, Classification.KeyValue},
                {SyntaxKind.UnterminatedString, Classification.StringLiteral},
                {SyntaxKind.UntilKeyword, Classification.Keyword},
                {SyntaxKind.WhileKeyword, Classification.Keyword},
            }.ToImmutableDictionary();

    }
}
