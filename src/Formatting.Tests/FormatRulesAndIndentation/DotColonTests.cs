// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests
{
    using Xunit;

    public class DotColonTests
    {
        private delegate void TestFunction(string original, string expected);
        private TestFunction GeneralTest = Tester.FormattingTest;

        [Fact]
        public void BasicDot()
        {
            string original = "t . foo ()";
            string expected = "t.foo ()";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void BasicColon()
        {
            string original = "t : foo ()";
            string expected = "t:foo ()";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void MultipleInOneLine()
        {
            string original = "t     :foo ().anothertable.  bar : foobar ()";
            string expected = "t:foo ().anothertable.bar:foobar ()";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void NothingToChange()
        {
            string original = "t:foo (); t.foo ()";
            this.GeneralTest(original, original);
        }

        [Fact]
        public void NewLineInBetweenDotOrColon()
        {
            string original = @"
t :
foo ()
t.
bar ()";
            string expected = @"
t:
foo ()
t.
bar ()";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void CommentInBetweenDotOrColon()
        {
            string original = "t. --[[comment]]bar; t: --[[comment]] bar";
            this.GeneralTest(original, original);
        }


    }
}
