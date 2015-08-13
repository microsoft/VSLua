using Xunit;

namespace Formatting.Tests
{
    public class ForLoopTests
    {
        delegate void TestFunction(string original, string expected);
        TestFunction GeneralTest = Tester.FormattingTest;

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
