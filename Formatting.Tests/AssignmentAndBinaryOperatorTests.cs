using Xunit;

namespace Formatting.Tests
{
    public class AssignmentAndBinaryOperatorTests
    {

        public void GeneralTestFunction(string original, string expected)
        {
            string actual = Tester.Format(original);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AssignmentBasic()
        {
            string original = "x=1";
            string expected = "x = 1";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        public void AssignmentTable()
        {
            string original = "{ x=1, y=2 }";
            string expected = "{ x = 1, y = 2 }";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        public void DoubleEquals()
        {
            string original = "x=1 == 1";
            string expected = "x = 1 == 1";
            GeneralTestFunction(original, expected);
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
            GeneralTestFunction(original, expected);
        }

        [Fact]
        public void MultiSpacedBinaryAssignment()
        {
            string original = "1     +       2        =       x";
            string expected = "1 + 2 = x";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        public void BasicBinaryOperator()
        {
            string original = "1+1";
            string expected = "1 + 1";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        public void MutliOperators()
        {
            string original = "1==1+2-4*10^6";
            string expected = "1 == 1 + 2 - 4 * 10 ^ 6";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        public void MutliLinedBinary()
        {
            string original = @"
1+1+
1";
            string expected = @"
1 + 1 +
1";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        public void BinaryAdjecent()
        {
            string original = "+-*/";
            string expected = "+ - * /";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        public void TrailingBinary()
        {
            string original = "1+";
            string expected = "1 +";
            GeneralTestFunction(original, expected);
        }

        [Fact]
        public void Mixed()
        {
            string original = "x +1 == 2   x= 3 /2+4";
            string expected = "x + 1 == 2    x = 3 / 2 + 4";
            GeneralTestFunction(original, expected);
        }

    }
}
