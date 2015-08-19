using Xunit;

namespace Formatting.Tests
{
    public class LeftCommaTests
    {

        delegate void TestFunction(string original, string expected);
        TestFunction GeneralTest = Tester.FormattingTest;

        [Fact]
        public void Comment()
        {
            string original = ",--[[ comment ]]x";
            GeneralTest(original, original);
        }

        [Fact]
        public void TwoVariables()
        {
            string original = "x,y = 1,2";
            string expected = "x, y = 1, 2";
            GeneralTest(original, expected);
        }

        [Fact]
        public void InTable()
        {
            string original = "t = { x,y,z }";
            string expected = "t = { x, y, z }";
            GeneralTest(original, expected);
        }

        [Fact]
        public void Parameters()
        {
            string original = "function foo(x,y,z) end";
            string expected = "function foo ( x, y, z ) end";
            GeneralTest(original, expected);
        }

        [Fact]
        public void TrailingComma()
        {
            string original = "t = { x, y,}";
            string expected = "t = { x, y, }";
            GeneralTest(original, expected);
        }

        [Fact]
        public void SpacesBeforeCommas1()
        {
            string original = "t = { x    ,y}";
            string expected = "t = { x, y }";
            GeneralTest(original, expected);
        }

        [Fact]
        public void SpacesBeforeCommas2()
        {
            string original = "t = { x    ,y          ,z}";
            string expected = "t = { x, y, z }";
            GeneralTest(original, expected);
        }

        [Fact]
        public void MultipleAssignment()
        {
            string original = "x,y = 1,2";
            string expected = "x, y = 1, 2";
            GeneralTest(original, expected);
        }

        [Fact]
        public void BrokenCode1()
        {
            string original = "x,= 1";
            string expected = "x, = 1";
            GeneralTest(original, expected);
        }

        [Fact]
        public void MoreSpacesBetweenComma()
        {
            string original = "x,             y = 1, 2";
            string expected = "x, y = 1, 2";
            GeneralTest(original, expected);
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
            GeneralTest(original, expected);
        }


    }
}
