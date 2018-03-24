// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests
{
    using Xunit;

    public class ForLoopTests
    {
        private delegate void TestFunction(string original, string expected);
        private TestFunction GeneralTest = Tester.FormattingTest;

        [Fact]
        public void Basic()
        {
            string original = "for i = 1, 2 do end";
            string expected = "for i = 1,2 do end";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void NoChange()
        {
            string original = "for i = 1,2 do end";
            string expected = "for i = 1,2 do end";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void Comment()
        {
            string original = "for i = 1,--[[ comment ]]2 do end";
            this.GeneralTest(original, original);
        }

        [Fact]
        public void ManySpaces()
        {
            string original = "for i = 1,                    2 do end";
            string expected = "for i = 1,2 do end";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void SpacesAssignment()
        {
            string original = "for i   =   1,2 do end";
            string expected = "for i = 1,2 do end";
            this.GeneralTest(original, expected);
        }
    }
}
