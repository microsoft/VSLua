using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                yield return new TagInfo(token.FullStart, token.Start - token.FullStart, Classification.Comment);
                yield return new TagInfo(token.Start, token.Length, SyntaxKindClassifications[token.Kind]);
            }
        }

        private static readonly ImmutableDictionary<SyntaxKind, Classification> SyntaxKindClassifications =
            new Dictionary<SyntaxKind, Classification>()
            {
                {SyntaxKind.AndBinop, Classification.Punctuation},
                {SyntaxKind.AssignmentOperator, Classification.Punctuation},
                {SyntaxKind.BitwiseAndOperator, Classification.Punctuation},
                {SyntaxKind.BitwiseLeftOperator, Classification.Punctuation},
                {SyntaxKind.BitwiseOrOperator, Classification.Punctuation},
                {SyntaxKind.BitwiseRightOperator, Classification.Punctuation},
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
                {SyntaxKind.EqualityOperator, Classification.Punctuation},
                {SyntaxKind.ExponentOperator, Classification.Punctuation},
                {SyntaxKind.FloorDivideOperator, Classification.Punctuation},
                {SyntaxKind.ForKeyword, Classification.Keyword},
                {SyntaxKind.FunctionKeyword, Classification.Keyword},
                {SyntaxKind.GotoKeyword, Classification.Keyword},
                {SyntaxKind.GreaterOrEqualOperator, Classification.Punctuation},
                {SyntaxKind.GreaterThanOperator, Classification.Punctuation},
                {SyntaxKind.Identifier, Classification.Identifier},
                {SyntaxKind.IfKeyword, Classification.Keyword},
                {SyntaxKind.InKeyword, Classification.Keyword},
                {SyntaxKind.LengthUnop, Classification.Punctuation},
                {SyntaxKind.LessOrEqualOperator, Classification.Punctuation},
                {SyntaxKind.LessThanOperator, Classification.Punctuation},
                {SyntaxKind.LocalKeyword, Classification.Keyword},
                {SyntaxKind.MinusOperator, Classification.Punctuation},
                {SyntaxKind.ModulusOperator, Classification.Punctuation},
                {SyntaxKind.MultiplyOperator, Classification.Punctuation},
                {SyntaxKind.NilKeyValue, Classification.Keyword},
                {SyntaxKind.NotEqualsOperator, Classification.Punctuation},
                {SyntaxKind.NotUnop, Classification.Punctuation},
                {SyntaxKind.Number, Classification.Number},
                {SyntaxKind.OpenBracket, Classification.Punctuation},
                {SyntaxKind.OpenCurlyBrace, Classification.Punctuation},
                {SyntaxKind.OpenParen, Classification.Punctuation},
                {SyntaxKind.OrBinop, Classification.Punctuation},
                {SyntaxKind.PlusOperator, Classification.Punctuation},
                {SyntaxKind.RepeatKeyword, Classification.Keyword},
                {SyntaxKind.ReturnKeyword, Classification.Keyword},
                {SyntaxKind.Semicolon, Classification.Punctuation},
                {SyntaxKind.String, Classification.StringLiteral},
                {SyntaxKind.StringConcatOperator, Classification.Punctuation},
                {SyntaxKind.ThenKeyword, Classification.Keyword},
                {SyntaxKind.TildeUnOp, Classification.Punctuation},
                {SyntaxKind.TrueKeyValue, Classification.Keyword},
                {SyntaxKind.UnterminatedString, Classification.StringLiteral},
                {SyntaxKind.UntilKeyword, Classification.Keyword},
                {SyntaxKind.WhileKeyword, Classification.Keyword}
            }.ToImmutableDictionary();

    }
}
