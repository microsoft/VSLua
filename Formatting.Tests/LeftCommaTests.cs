using Xunit;

namespace Formatting.Tests
{
    public class LeftCommaTests
    {

        internal void GeneralTest(string original, string expected)
        {
            string actual = Tester.Format(original);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Basic()
        {
            string original = ",x";
            string expected = ", x";
            GeneralTest(original, expected);
        }

        [Fact]
        public void Comment()
        {
            string original = ",--[[ comment ]]x";
            GeneralTest(original, original);
        }

        [Fact]
        public void TwoVariables()
        {
            string original = "x,y";
            string expected = "x, y";
            GeneralTest(original, expected);
        }

        [Fact]
        public void InTable()
        {
            string original = "{ x,y,z }";
            string expected = "{ x, y, z }";
            GeneralTest(original, expected);
        }

        [Fact]
        public void Parameters()
        {
            string original = "function foo(x,y,z)";
            string expected = "function foo(x, y, z)";
            GeneralTest(original, expected);
        }

        [Fact]
        public void TrailingComma()
        {
            string original = "{ x, y,}";
            string expected = "{ x, y, }";
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
        public void BrokenCode2()
        {
            string original = "{ x,,y }";
            string expected = "{ x, , y }";
            GeneralTest(original, expected);
        }

        [Fact]
        public void ManyTestsInOne()
        {
            string original = "{ x,y, ,,,, },y32,,,s2,";
            string expected = "{ x, y, , , , , }, y32, , , s2,";
            GeneralTest(original, expected);
        }

        [Fact]
        public void MoreSpacesBetweenComma()
        {
            string original = "x,             y";
            string expected = "x, y";
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
