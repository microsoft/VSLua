using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting
{
    internal static class TokenRange
    {
        internal static readonly ImmutableArray<TokenType> AnyVisible =
            TokenRange.Fill(
                Enum.GetValues(typeof(TokenType)),
                new TokenType[] { TokenType.EndOfFile, TokenType.Unknown }).ToImmutableArray();

        internal static readonly ImmutableArray<TokenType> All =
            TokenRange.Fill(
                Enum.GetValues(typeof(TokenType)),
                new TokenType[] { }).ToImmutableArray();

        internal static readonly ImmutableArray<TokenType> BinaryOperators =
            new List<TokenType>
            {
                TokenType.AndBinop,
                TokenType.BitwiseAndOperator,
                TokenType.BitwiseLeftOperator,
                TokenType.BitwiseOrOperator,
                TokenType.BitwiseRightOperator,
                TokenType.EqualityOperator,
                TokenType.ExponentOperator,
                TokenType.FloorDivideOperator,
                TokenType.DivideOperator,
                TokenType.GreaterOrEqualOperator,
                TokenType.GreaterThanOperator,
                TokenType.LessOrEqualOperator,
                TokenType.LessThanOperator,
                TokenType.MinusOperator,
                TokenType.ModulusOperator,
                TokenType.MultiplyOperator,
                TokenType.NotEqualsOperator,
                TokenType.OrBinop,
                TokenType.PlusOperator,
                TokenType.StringConcatOperator,
                TokenType.TildeUnOp,
                TokenType.VarArgOperator,
            }.ToImmutableArray();

        internal static readonly ImmutableArray<TokenType> Brackets = new List<TokenType>
        {
            TokenType.OpenBracket,
            TokenType.CloseBracket,
            TokenType.OpenCurlyBrace,
            TokenType.CloseCurlyBrace,
            TokenType.OpenParen,
            TokenType.CloseParen,
        }.ToImmutableArray();

        internal static readonly ImmutableArray<TokenType> Value = new List<TokenType>
        {
            TokenType.Number,
            TokenType.String,
            TokenType.FalseKeyValue,
            TokenType.TrueKeyValue,
            TokenType.NilKeyValue,
            TokenType.Identifier,
            TokenType.FunctionKeyword
        }.ToImmutableArray();

        private static List<TokenType> Fill(Array values, TokenType[] exclude)
        {
            List<TokenType> tokenTypes = new List<TokenType>();

            foreach (TokenType tokenType in values)
            {
                if (!exclude.Contains(tokenType))
                {
                    tokenTypes.Add(tokenType);
                }
            }
            return tokenTypes;
        }
    }
}
