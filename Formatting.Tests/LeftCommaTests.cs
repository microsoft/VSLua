using Xunit;

namespace Formatting.Tests
{
    public class LeftCommaTests
    {

        internal void GeneralRuleTest(string original, string expected)
        {
            string actual = Tester.Format(original);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Basic()
        {
            string original = ",x";
            string expected = ", x";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        public void TwoVariables()
        {
            string original = "x,y";
            string expected = "x, y";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        public void InTable()
        {
            string original = "{ x,y,z }";
            string expected = "{ x, y, z }";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        public void Parameters()
        {
            string original = "function foo(x,y,z)";
            string expected = "function foo(x, y, z)";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        public void TrailingComma()
        {
            string original = "{ x, y,}";
            string expected = "{ x, y, }";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        public void MultipleAssignment()
        {
            string original = "x,y = 1,2";
            string expected = "x, y = 1, 2";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        public void BrokenCode1()
        {
            string original = "x,= 1";
            string expected = "x, = 1";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        public void BrokenCode2()
        {
            string original = "{ x,,y }";
            string expected = "{ x, , y }";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        public void ManyTestsInOne()
        {
            string original = "{ x,y, ,,,, },y32,,,s2,";
            string expected = "{ x, y, , , , , }, y32, , , s2,";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        public void MoreSpacesBetweenComma()
        {
            string original = "x,             y";
            string expected = "x, y";
            GeneralRuleTest(original, expected);
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
            GeneralRuleTest(original, expected);
        }


    }
}
