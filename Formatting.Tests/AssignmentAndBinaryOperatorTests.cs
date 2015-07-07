using Xunit;

namespace Formatting.Tests
{
    internal static class AssignmentAndBinaryOperatorTests
    {

        internal static void GeneralTestFunction(string original, string expected)
        {
            string actual = Tester.Format(original);
            Assert.Equal(actual, expected);
        }

        [Fact]
        internal static void AssignmentBasic()
        {
            string original = "x=1";
            string expected = "x = 1";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        internal static void AssignmentTable()
        {
            string original = "{ x=1, y=2 }";
            string expected = "{ x = 1, y = 2 }";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        internal static void DoubleEquals()
        {
            string original = "x=1 == 1";
            string expected = "x = 1 == 1";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        internal static void MultiLinedAssignemnt()
        {
            string original = @"
x=
1";
            string expected = @"
x =
1";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        internal static void MultiSpacedBinaryAssignment()
        {
            string original = "1     +       2        =       x";
            string expected = "1 + 2 = x";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        internal static void BasicBinaryOperator()
        {
            string original = "1+1";
            string expected = "1 + 1";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        internal static void MutliOperators()
        {
            string original = "1==1+2-4*10^6";
            string expected = "1 == 1 + 2 - 4 * 10 ^ 6";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        internal static void MutliLinedBinary()
        {
            string original = @"
1+1+
1";
            string expected = @"
1 + 1 +
1";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        internal static void BinaryAdjecent()
        {
            string original = "+-*/";
            string expected = "+ - * /";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        internal static void TrailingBinary()
        {
            string original = "1+";
            string expected = "1 +";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        internal static void Mixed()
        {
            string original = "x +1 == 2   x= 3 /2+4";
            string expected = "x + 1 == 2    x = 3 / 2 + 4";
            GeneralTestFunction(original, expected);
        }

    }
}
