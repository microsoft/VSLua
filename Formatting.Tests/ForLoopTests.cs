using Xunit;

namespace Formatting.Tests
{
    public class ForLoopTests
    {
        public void GeneralTest(string original, string expected)
        {
            string actual = Tester.Format(original);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Basic()
        {
            string original = "for i = 1, 2 do end";
            string expected = "for i = 1,2 do end";
            GeneralTest(original, expected);
        }

        [Fact]
        public void NoChange()
        {
            string original = "for i = 1,2 do end";
            string expected = "for i = 1,2 do end";
            GeneralTest(original, expected);
        }

        [Fact]
        public void Comment()
        {
            string original = "for i = 1,--[[ comment ]]2 do end";
            GeneralTest(original, original);
        }

        [Fact]
        public void ManySpaces()
        {
            string original = "for i = 1,                    2 do end";
            string expected = "for i = 1,2 do end";
            GeneralTest(original, expected);
        }
    }
}
