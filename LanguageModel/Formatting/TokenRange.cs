using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting
{
    internal static class TokenRange
    {
        internal static List<SyntaxKind> AnyVisible =
            TokenRange.Fill(
                Enum.GetValues(typeof(SyntaxKind)),
                new SyntaxKind[] { SyntaxKind.EndOfFile, SyntaxKind.Unknown });

        internal static List<SyntaxKind> All =
            TokenRange.Fill(
                Enum.GetValues(typeof(SyntaxKind)),
                new SyntaxKind[] { }); // Might need to add TokenType.Unknown to skip

        internal static List<SyntaxKind> BinaryOperators =
            new List<SyntaxKind>
            {
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
                SyntaxKind.VarArgOperator,
            };

        internal static List<SyntaxKind> Brackets = new List<SyntaxKind>
        {
            SyntaxKind.OpenBracket,
            SyntaxKind.CloseBracket,
            SyntaxKind.OpenCurlyBrace,
            SyntaxKind.CloseCurlyBrace,
            SyntaxKind.OpenParen,
            SyntaxKind.CloseParen,
        };

        internal static List<SyntaxKind> Value = new List<SyntaxKind>
        {
            SyntaxKind.Number,
            SyntaxKind.String,
            SyntaxKind.FalseKeyValue,
            SyntaxKind.TrueKeyValue,
            SyntaxKind.NilKeyValue,
            SyntaxKind.Identifier,
        };

        private static List<SyntaxKind> Fill(Array values, SyntaxKind[] exclude)
        {

            List<SyntaxKind> tokenTypes = new List<SyntaxKind>();

            foreach (SyntaxKind tokenType in values)
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
