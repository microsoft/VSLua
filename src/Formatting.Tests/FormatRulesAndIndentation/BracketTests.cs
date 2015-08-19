using Xunit;

namespace Formatting.Tests
{
    public class BracketTests
    {

        delegate void TestFunction(string original, string expected);
        TestFunction FormattingTest = Tester.FormattingTest;

        [Fact]
        public void BasicSquare()
        {
            string original = "t[1] = 10";
            string expected = "t[ 1 ] = 10";
            FormattingTest(original, expected);
        }

        [Fact]
        public void BasicParenthesis()
        {
            string original = "function foo (x) end";
            string expected = "function foo ( x ) end";
            FormattingTest(original, expected);
        }

        [Fact]
        public void BasicCurly()
        {
            string original = "t = {1}";
            string expected = "t = { 1 }";
            FormattingTest(original, expected);
        }

        [Fact]
        public void CommentCurly()
        {
            string original = "t = { 1--[[ comment ]]}";
            FormattingTest(original, original);
        }

        [Fact]
        public void CommentSquare()
        {
            string original = "t = [ 1--[[ comment ]]]";
            FormattingTest(original, original);
        }

        [Fact]
        public void CommentParen()
        {
            string original = "foo ( 1--[[ comment ]])";
            FormattingTest(original, original);
        }

        [Fact]
        public void LongTable()
        {
            string original = "t = {1, 3, 4, 5, 6, 7,}";
            string expected = "t = { 1, 3, 4, 5, 6, 7, }";
            FormattingTest(original, expected);
        }

        [Fact]
        public void LongParams()
        {
            string original = "foo = function (x, y, z, w) end";
            string expected = "foo = function ( x, y, z, w ) end";
            FormattingTest(original, expected);
        }

        [Fact]
        public void LongSquare()
        {
            string original = "t[\"this is a test that grabs this key in Lua\"] = 10";
            string expected = "t[ \"this is a test that grabs this key in Lua\" ] = 10";
            FormattingTest(original, expected);
        }

        [Fact]
        public void BracketAfterFunction()
        {
            string original = "foo = function     () end";
            string expected = "foo = function () end";
            FormattingTest(original, expected);
        }

        [Fact]
        public void BracketAfterFunction2()
        {
            string original = "function foo      () end";
            string expected = "function foo () end";
            FormattingTest(original, expected);
        }

        [Fact]
        public void SquareBracketAfterAnything()
        {
            string original = "x = x     [1]";
            string expected = "x = x[ 1 ]";
            FormattingTest(original, expected);
        }

        [Fact]
        public void CommaBeforeCloseCurly()
        {
            string original = "x = { x,}";
            string expected = "x = { x, }";
            FormattingTest(original, expected);
        }

        [Fact]
        public void CurlyBracesAfterAnything()
        {
            string original = "foo   {}";
            string expected = "foo {}";
            FormattingTest(original, expected);
        }

        [Fact]
        public void MultiLinedTable()
        {
            string original = @"
t = {
    1,
    2,
}";
            FormattingTest(original, original);
        }

        [Fact]
        public void MutliLinedSquare()
        {
            string original = @"
t = [


1


] = 10";
            FormattingTest(original, original);
        }

        [Fact]
        public void MutliLinedParams()
        {
            string original = @"
foo = function (
a, b, c) end";
            string expected = @"
foo = function (
a, b, c ) end";
            FormattingTest(original, expected);
        }

        [Fact]
        public void ClosedSquare()
        {
            string original = "](";
            string expected = "](";
            FormattingTest(original, expected);
        }

        [Fact]
        public void ClosedParen()
        {
            string original = ")(";
            string expected = ")(";
            FormattingTest(original, expected);
        }

        [Fact]
        public void ClosedCurly()
        {
            string original = "}(";
            string expected = "}(";
            FormattingTest(original, expected);
        }

    }
}
