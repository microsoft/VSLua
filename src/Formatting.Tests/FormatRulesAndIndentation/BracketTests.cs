// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests
{
    using Xunit;

    public class BracketTests
    {

        private delegate void TestFunction(string original, string expected);
        private TestFunction FormattingTest = Tester.FormattingTest;

        [Fact]
        public void BasicSquare()
        {
            string original = "t[1] = 10";
            string expected = "t[ 1 ] = 10";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void BasicParenthesis()
        {
            string original = "function foo (x) end";
            string expected = "function foo ( x ) end";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void BasicCurly()
        {
            string original = "t = {1}";
            string expected = "t = { 1 }";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void CommentCurly()
        {
            string original = "t = { 1--[[ comment ]]}";
            this.FormattingTest(original, original);
        }

        [Fact]
        public void CommentSquare()
        {
            string original = "t = [ 1--[[ comment ]]]";
            this.FormattingTest(original, original);
        }

        [Fact]
        public void CommentParen()
        {
            string original = "foo ( 1--[[ comment ]])";
            this.FormattingTest(original, original);
        }

        [Fact]
        public void LongTable()
        {
            string original = "t = {1, 3, 4, 5, 6, 7,}";
            string expected = "t = { 1, 3, 4, 5, 6, 7, }";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void LongParams()
        {
            string original = "foo = function (x, y, z, w) end";
            string expected = "foo = function ( x, y, z, w ) end";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void LongSquare()
        {
            string original = "t[\"this is a test that grabs this key in Lua\"] = 10";
            string expected = "t[ \"this is a test that grabs this key in Lua\" ] = 10";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void BracketAfterFunction()
        {
            string original = "foo = function     () end";
            string expected = "foo = function () end";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void BracketAfterFunction2()
        {
            string original = "function foo      () end";
            string expected = "function foo () end";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void SquareBracketAfterAnything()
        {
            string original = "x = x     [1]";
            string expected = "x = x[ 1 ]";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void CommaBeforeCloseCurly()
        {
            string original = "x = { x,}";
            string expected = "x = { x, }";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void CurlyBracesAfterAnything()
        {
            string original = "foo   {}";
            string expected = "foo {}";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void MultiLinedTable()
        {
            string original = @"
t = {
    1,
    2,
}";
            this.FormattingTest(original, original);
        }

        [Fact]
        public void MutliLinedSquare()
        {
            string original = @"
t = [


1


] = 10";
            this.FormattingTest(original, original);
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
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void ClosedSquare()
        {
            string original = "](";
            string expected = "](";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void ClosedParen()
        {
            string original = ")(";
            string expected = ")(";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void ClosedCurly()
        {
            string original = "}(";
            string expected = "}(";
            this.FormattingTest(original, expected);
        }

    }
}
