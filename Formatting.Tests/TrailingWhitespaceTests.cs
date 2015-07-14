using Xunit;

namespace Formatting.Tests
{
    public class TrailingWhitespaceTests
    {
        delegate void TestFunction(string original, string expected);
        TestFunction GeneralTest = Tester.GeneralTest;


    }
}
