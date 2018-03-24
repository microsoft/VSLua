// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests
{
    using Xunit;

    public class LeftCommaTests
    {

        private delegate void TestFunction(string original, string expected);
        private TestFunction GeneralTest = Tester.FormattingTest;

        [Fact]
        public void Comment()
        {
            string original = ",--[[ comment ]]x";
            this.GeneralTest(original, original);
        }

        [Fact]
        public void TwoVariables()
        {
            string original = "x,y = 1,2";
            string expected = "x, y = 1, 2";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void InTable()
        {
            string original = "t = { x,y,z }";
            string expected = "t = { x, y, z }";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void Parameters()
        {
            string original = "function foo(x,y,z) end";
            string expected = "function foo ( x, y, z ) end";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void TrailingComma()
        {
            string original = "t = { x, y,}";
            string expected = "t = { x, y, }";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void SpacesBeforeCommas1()
        {
            string original = "t = { x    ,y}";
            string expected = "t = { x, y }";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void SpacesBeforeCommas2()
        {
            string original = "t = { x    ,y          ,z}";
            string expected = "t = { x, y, z }";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void MultipleAssignment()
        {
            string original = "x,y = 1,2";
            string expected = "x, y = 1, 2";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void BrokenCode1()
        {
            string original = "x,= 1";
            string expected = "x, = 1";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void MoreSpacesBetweenComma()
        {
            string original = "x,             y = 1, 2";
            string expected = "x, y = 1, 2";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void MultiLined()
        {
            string original = @"
x,
y = 1,2";
            string expected = @"
x,
y = 1, 2";
            this.GeneralTest(original, expected);
        }


    }
}
