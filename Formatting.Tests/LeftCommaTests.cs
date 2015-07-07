using Xunit;

namespace Formatting.Tests
{
    internal static class LeftCommaTests
    {

        internal static void GeneralRuleTest(string original, string expected)
        {
            string actual = Tester.Format(original);
            Assert.Equal(actual, expected);
        }

        [Fact]
        internal static void Basic()
        {
            string original = ",x";
            string expected = ", x";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        internal static void TwoVariables()
        {
            string original = "x,y";
            string expected = "x, y";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        internal static void InTable()
        {
            string original = "{ x,y,z }";
            string expected = "{ x, y, z }";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        internal static void Parameters()
        {
            string original = "function foo(x,y,z)";
            string expected = "function foo(x, y, z)";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        internal static void TrailingComma()
        {
            string original = "{ x, y,}";
            string expected = "{ x, y, }";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        internal static void MultipleAssignment()
        {
            string original = "x,y = 1,2";
            string expected = "x, y = 1, 2";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        internal static void BrokenCode1()
        {
            string original = "x,= 1";
            string expected = "x, = 1";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        internal static void BrokenCode2()
        {
            string original = "{ x,,y }";
            string expected = "{ x, , y }";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        internal static void ManyTestsInOne()
        {
            string original = "{ x,y, ,,,, },y32,,,s2,";
            string expected = "{ x, y, , , , , }, y32, , , s2,";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        internal static void MoreSpacesBetweenComma()
        {
            string original = "x,             y";
            string expected = "x, y";
            GeneralRuleTest(original, expected);
        }

        [Fact]
        internal static void MultiLined()
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
