using Xunit;

namespace Formatting.Tests
{
    internal static class ForLoopTests
    {
        internal static void GeneralFunctionTest(string original, string expected)
        {
            string actual = Tester.Format(original);
            Assert.Equal(actual, expected);
        }

        [Fact]
        internal static void Basic()
        {
            string original = "for i = 1, 2 do end";
            string expected = "for i = 1,2 do end";
            Assert.Equal(original, expected);
        }

        [Fact]
        internal static void NoChange()
        {
            string original = "for i = 1,2 do end";
            string expected = "for i = 1,2 do end";
            Assert.Equal(original, expected);
        }

        [Fact]
        internal static void ManySpaces()
        {
            string original = "for i = 1,                    2 do end";
            string expected = "for i = 1,2 do end";
            Assert.Equal(original, expected);
        }
    }
}
