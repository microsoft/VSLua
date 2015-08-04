using System;
using System.Collections.Immutable;
using System.Linq;
using Validation;

namespace LanguageService.Formatting
{
    internal static class TokenRange
    {
        internal static readonly ImmutableArray<TokenType> AnyVisible =
            TokenRange.Fill(
                Enum.GetValues(typeof(SyntaxKind)),
                new SyntaxKind[] { SyntaxKind.EndOfFile, SyntaxKind.Unknown });

        internal static readonly ImmutableArray<TokenType> All =
            TokenRange.Fill(
                Enum.GetValues(typeof(TokenType)),
                new TokenType[] { });

        internal static readonly ImmutableArray<TokenType> BinaryOperators = ImmutableArray.Create
        (
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
            TokenType.VarArgOperator
        );

        internal static readonly ImmutableArray<TokenType> Brackets = ImmutableArray.Create
        (
            TokenType.OpenBracket,
            TokenType.CloseBracket,
            TokenType.OpenCurlyBrace,
            TokenType.CloseCurlyBrace,
            TokenType.OpenParen,
            TokenType.CloseParen
        );

        internal static readonly ImmutableArray<TokenType> Value = ImmutableArray.Create(
            TokenType.Number,
            TokenType.String,
            TokenType.FalseKeyValue,
            TokenType.TrueKeyValue,
            TokenType.NilKeyValue,
            TokenType.Identifier,
            TokenType.FunctionKeyword
        );

        private static ImmutableArray<TokenType> Fill(Array values, TokenType[] exclude)
        {
            Requires.NotNull(values, nameof(values));
            Requires.NotNull(exclude, nameof(exclude));

            var tokenTypes = ImmutableArray.CreateBuilder<TokenType>(values.Length);

            foreach (SyntaxKind tokenType in values)
            {
                if (!exclude.Contains(tokenType))
                {
                    tokenTypes.Add(tokenType);
                }
            }

            return tokenTypes.ToImmutable();
        }
    }
}
