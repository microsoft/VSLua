using Xunit;


namespace Formatting.Tests
{
    public class AssignmentAndBinaryOperatorTests
    {

        delegate void TestFunction(string original, string expected);
        TestFunction GeneralTest = Tester.FormattingTest;

        [Fact]
        public void AssignmentBasic()
        {
            string original = "x=1";
            string expected = "x = 1";
            GeneralTest(original, expected);
        }

        [Fact]
        public void AssignmentComment()
        {
            string original = "x =--[[ comment ]]1";
            GeneralTest(original, original);
        }

        [Fact]
        public void BasicMinus()
        {
            string original = "-hello_world";
            string expected = "- hello_world";
            GeneralTest(original, expected);
        }

        [Fact]
        public void AssignmentTable()
        {
            string original = "t = { x=1, y=2 }";
            string expected = "t = { x = 1, y = 2 }";
            GeneralTest(original, expected);
        }

        [Fact]
        public void DoubleEquals()
        {
            string original = "x=1 == 1";
            string expected = "x = 1 == 1";
            GeneralTest(original, expected);
        }

        [Fact]
        public void Concat()
        {
            string original = "x..y";
            string expected = "x .. y";
            GeneralTest(original, expected);
        }

        [Fact]
        public void BinaryComment()
        {
            string original = "x ==--[[ comment ]]y";
            GeneralTest(original, original);
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
            GeneralTest(original, expected);
        }

        [Fact]
        public void MultiSpacedBinaryAssignment()
        {
            string original = "1     +       2        =       x";
            string expected = "1 + 2 = x";
            GeneralTest(original, expected);
        }

        [Fact]
        public void BasicBinaryOperator()
        {
            string original = "x = 1+1";
            string expected = "x = 1 + 1";
            GeneralTest(original, expected);
        }

        [Fact]
        public void MutliOperators()
        {
            string original = "x = 1==1+2-4*10^6";
            string expected = "x = 1 == 1 + 2 - 4 * 10 ^ 6";
            GeneralTest(original, expected);
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
            GeneralTest(original, expected);
        }

        [Fact]
        public void BinaryAdjecent()
        {
            string original = "+-*/";
            string expected = "+-*/";
            GeneralTest(original, expected);
        }

        [Fact]
        public void TrailingBinary()
        {
            string original = "x = 1+";
            string expected = "x = 1 +";
            GeneralTest(original, expected);
        }

        [Fact]
        public void Mixed()
        {
            string original = "x +1 == 2   x= 3 /2+4";
            string expected = "x + 1 == 2   x = 3 / 2 + 4";
            GeneralTest(original, expected);
        }

    }
}
