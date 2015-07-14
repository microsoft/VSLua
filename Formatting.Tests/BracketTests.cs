using Xunit;

namespace Formatting.Tests
{
    public class BracketTests
    {

        delegate void TestFunction(string original, string expected);
        TestFunction GeneralTest = Tester.GeneralTest;

        [Fact]
        public void BasicSquare()
        {
            string original = "t[1]";
            string expected = "t[ 1 ]";
            GeneralTest(original, expected);
        }

        [Fact]
        public void BasicParenthesis()
        {
            string original = "function foo (x) end";
            string expected = "function foo ( x ) end";
            GeneralTest(original, expected);
        }

        [Fact]
        public void BasicCurly()
        {
            string original = "t = {1}";
            string expected = "t = { 1 }";
            GeneralTest(original, expected);
        }

        [Fact]
        public void CommentCurly()
        {
            string original = "t = {1--[[ comment ]]}";
            GeneralTest(original, original);
        }

        [Fact]
        public void CommentSquare()
        {
            string original = "t = [1--[[ comment ]]]";
            GeneralTest(original, original);
        }

        [Fact]
        public void CommentParen()
        {
            string original = "foo(1--[[ comment ]])";
            GeneralTest(original, original);
        }

        [Fact]
        public void LongTable()
        {
            string original = "t = {1, 3, 4, 5, 6, 7,}";
            string expected = "t = { 1, 3, 4, 5, 6, 7, }";
            GeneralTest(original, expected);
        }

        [Fact]
        public void LongParams()
        {
            string original = "foo = function(x, y, z, w) end";
            string expected = "foo = function( x, y, z, w ) end";
            GeneralTest(original, expected);
        }

        [Fact]
        public void LongSquare()
        {
            string original = "t[\"this is a test that grabs this key in Lua\"]";
            string expected = "t[ \"this is a test that grabs this key in Lua\" ]";
            GeneralTest(original, expected);
        }

        [Fact]
        public void MultiLinedTable()
        {
            string original = @"
t = {
    1,
    2,
}";
            GeneralTest(original, original);
        }

        [Fact]
        public void MutliLinedSquare()
        {
            string original = @"
t = [


1


]";
            GeneralTest(original, original);
        }

        [Fact]
        public void MutliLinedParams()
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
        public void ClosedSquare()
        {
            string original = "](";
            string expected = "](";
            GeneralTest(original, expected);
        }

        [Fact]
        public void ClosedParen()
        {
            string original = ")(";
            string expected = ")(";
            GeneralTest(original, expected);
        }

        [Fact]
        public void ClosedCurly()
        {
            string original = "}(";
            string expected = "}(";
            GeneralTest(original, expected);
        }

    }
}
