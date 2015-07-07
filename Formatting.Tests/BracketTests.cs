using Xunit;

namespace Formatting.Tests
{
    internal static class BracketTests
    {

        internal static void GeneralTest(string original, string expected)
        {
            string actual = Tester.Format(original);
            Assert.Equal(actual, expected);
        }

        [Fact]
        internal static void BasicSquare()
        {
            string original = "t[1]";
            string expected = "t[ 1 ]";
            GeneralTest(original, expected);
        }

        [Fact]
        internal static void BasicParenthesis()
        {
            string original = "function foo (x) end";
            string expected = "function foo ( x ) end";
            GeneralTest(original, expected);
        }

        [Fact]
        internal static void BasicCurly()
        {
            string original = "t = {1}";
            string expected = "t = { 1 }";
            GeneralTest(original, expected);
        }

        [Fact]
        internal static void LongTable()
        {
            string original = "t = {1, 3, 4, 5, 6, 7,}";
            string expected = "t = { 1, 3, 4, 5, 6, 7, }";
            GeneralTest(original, expected);
        }

        [Fact]
        internal static void LongParams()
        {
            string original = "foo = function(x, y, z, w) end";
            string expected = "foo = function( x, y, z, w ) end";
            GeneralTest(original, expected);
        }

        [Fact]
        internal static void LongSquare()
        {
            string original = "t[\"this is a test that grabs this key in Lua\"]";
            string expected = "t[ \"this is a test that grabs this key in Lua\" ]";
            GeneralTest(original, expected);
        }

        [Fact]
        internal static void MultiLinedTable()
        {
            string original = @"
t = {
    1,
    2,
}";
            GeneralTest(original, original);
        }

        [Fact]
        internal static void MutliLinedSquare()
        {
            string original = @"
t = [


1


]";
            GeneralTest(original, original);
        }

        [Fact]
        internal static void MutliLinedParams()
        {
            string original = @"
foo = function (
a, b, c)";
            string expected = @"
foo = function (
a, b, c )";
            GeneralTest(original, expected);
        }

        [Fact]
        internal static void ClosedSquare()
        {
            string original = "](";
            string expected = "] (";
            GeneralTest(original, expected);
        }

        [Fact]
        internal static void ClosedParen()
        {
            string original = ")(";
            string expected = ") (";
            GeneralTest(original, expected);
        }

        [Fact]
        internal static void ClosedCurly()
        {
            string original = "}(";
            string expected = "} (";
            GeneralTest(original, expected);
        }

    }
}
