using System;
using System.Collections.Immutable;
using System.Linq;
using Validation;

namespace LanguageService.Formatting
{
    internal static class TokenRange
    {
        internal static readonly ImmutableArray<SyntaxKind> AnyVisible =
            TokenRange.Fill(
                Enum.GetValues(typeof(SyntaxKind)),
                new SyntaxKind[] { SyntaxKind.EndOfFile, SyntaxKind.Unknown });

        internal static readonly ImmutableArray<SyntaxKind> All =
            TokenRange.Fill(
                Enum.GetValues(typeof(SyntaxKind)),
                new SyntaxKind[] { });

        internal static readonly ImmutableArray<SyntaxKind> BinaryOperators = ImmutableArray.Create
        (
            SyntaxKind.AndBinop,
            SyntaxKind.BitwiseAndOperator,
            SyntaxKind.BitwiseLeftOperator,
            SyntaxKind.BitwiseOrOperator,
            SyntaxKind.BitwiseRightOperator,
            SyntaxKind.EqualityOperator,
            SyntaxKind.ExponentOperator,
            SyntaxKind.FloorDivideOperator,
            SyntaxKind.DivideOperator,
            SyntaxKind.GreaterOrEqualOperator,
            SyntaxKind.GreaterThanOperator,
            SyntaxKind.LessOrEqualOperator,
            SyntaxKind.LessThanOperator,
            SyntaxKind.MinusOperator,
            SyntaxKind.ModulusOperator,
            SyntaxKind.MultiplyOperator,
            SyntaxKind.NotEqualsOperator,
            SyntaxKind.OrBinop,
            SyntaxKind.PlusOperator,
            SyntaxKind.StringConcatOperator,
            SyntaxKind.TildeUnOp,
            SyntaxKind.VarArgOperator
        );

        internal static readonly ImmutableArray<SyntaxKind> Brackets = ImmutableArray.Create
        (
            SyntaxKind.OpenBracket,
            SyntaxKind.CloseBracket,
            SyntaxKind.OpenCurlyBrace,
            SyntaxKind.CloseCurlyBrace,
            SyntaxKind.OpenParen,
            SyntaxKind.CloseParen
        );

        internal static readonly ImmutableArray<SyntaxKind> Value = ImmutableArray.Create(
            SyntaxKind.Number,
            SyntaxKind.String,
            SyntaxKind.FalseKeyValue,
            SyntaxKind.TrueKeyValue,
            SyntaxKind.NilKeyValue,
            SyntaxKind.Identifier,
            SyntaxKind.FunctionKeyword
        );

        private static ImmutableArray<SyntaxKind> Fill(Array values, SyntaxKind[] exclude)
        {
            Requires.NotNull(values, nameof(values));
            Requires.NotNull(exclude, nameof(exclude));

            var SyntaxKinds = ImmutableArray.CreateBuilder<SyntaxKind>(values.Length);

            foreach (SyntaxKind SyntaxKind in values)
            {
                if (!exclude.Contains(SyntaxKind))
                {
                    SyntaxKinds.Add(SyntaxKind);
                }
            }

            return SyntaxKinds.ToImmutable();
        }
    }
}
