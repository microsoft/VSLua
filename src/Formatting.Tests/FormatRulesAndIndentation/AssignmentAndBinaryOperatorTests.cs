using Xunit;


namespace Formatting.Tests
{
    public class AssignmentAndBinaryOperatorTests
    {

        delegate void TestFunction(string original, string expected);
        TestFunction FormattingTest = Tester.FormattingTest;

        [Fact]
        public void AssignmentBasic()
        {
            string original = "x=1";
            string expected = "x = 1";
            FormattingTest(original, expected);
        }

        [Fact]
        public void AssignmentComment()
        {
            string original = "x =--[[ comment ]]1";
            FormattingTest(original, original);
        }

        [Fact(Skip = "Weird")]
        public void BasicMinus()
        {
            string original = "x = -hello_world";
            string expected = "x = - hello_world";
            FormattingTest(original, expected);
        }

        [Fact]
        public void AssignmentTable()
        {
            string original = "t = { x=1, y=2 }";
            string expected = "t = { x = 1, y = 2 }";
            FormattingTest(original, expected);
        }

        [Fact]
        public void DoubleEquals()
        {
            string original = "x=1 == 1";
            string expected = "x = 1 == 1";
            FormattingTest(original, expected);
        }

        [Fact]
        public void Concat()
        {
            string original = "x = x..y";
            string expected = "x = x .. y";
            FormattingTest(original, expected);
        }

        [Fact]
        public void BinaryComment()
        {
            string original = "x ==--[[ comment ]]y";
            FormattingTest(original, original);
        }

        [Fact]
        public void MultiLinedAssignemnt()
        {
            string original = @"
x=
1";
            string expected = @"
x =
1";
            FormattingTest(original, expected);
        }

        [Fact]
        public void MultiSpacedBinaryAssignment()
        {
            string original = "x     =       2         +      1";
            string expected = "x = 2 + 1";
            FormattingTest(original, expected);
        }

        [Fact]
        public void BasicBinaryOperator()
        {
            string original = "x = 1+1";
            string expected = "x = 1 + 1";
            FormattingTest(original, expected);
        }

        [Fact]
        public void MutliOperators()
        {
            string original = "x = 1==1+2-4*10^6";
            string expected = "x = 1 == 1 + 2 - 4 * 10 ^ 6";
            FormattingTest(original, expected);
        }

        [Fact]
        public void MutliLinedBinary()
        {
            string original = @"
x = 1+1+
1";
            string expected = @"
x = 1 + 1 +
1";
            FormattingTest(original, expected);
        }

        [Fact]
        public void BinaryAdjecent()
        {
            string original = "+-*/";
            string expected = "+-*/";
            FormattingTest(original, expected);
        }

        [Fact]
        public void TrailingBinary()
        {
            string original = "x = 1+";
            FormattingTest(original, original);
        }

        [Fact]
        public void Mixed()
        {
            string original = "x = x +1 == 2   x= 3 /2+4";
            string expected = "x = x + 1 == 2   x = 3 / 2 + 4";
            FormattingTest(original, expected);
        }

        [Fact]
        public void TableAssignment()
        {
            string original = "t = { x=y }";
            string expected = "t = { x = y }";
            FormattingTest(original, expected);
        }
    }
}
