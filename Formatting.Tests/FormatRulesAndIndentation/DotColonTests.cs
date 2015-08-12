using Xunit;

namespace Formatting.Tests
{
    public class DotColonTests
    {
        delegate void TestFunction(string original, string expected);
        TestFunction GeneralTest = Tester.GeneralTest;

        [Fact]
        public void BasicDot()
        {
            string original = "t . foo";
            string expected = "t.foo";
            GeneralTest(original, expected);
        }

        [Fact]
        public void BasicColon()
        {
            string original = "t : foo";
            string expected = "t:foo";
            GeneralTest(original, expected);
        }

        [Fact]
        public void MultipleInOneLine()
        {
            string original = "t     :foo ().anothertable.  bar : foobar ()";
            string expected = "t:foo ().anothertable.bar:foobar ()";
            GeneralTest(original, expected);
        }

        [Fact]
        public void NothingToChange()
        {
            string original = "t:foo (); t.foo ()";
            GeneralTest(original, original);
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
            GeneralTest(original, expected);
        }

        [Fact]
        public void CommentInBetweenDotOrColon()
        {
            string original = "t. --[[comment]]bar; t: --[[comment]] bar";
            GeneralTest(original, original);
        }


    }
}
