using Xunit;
using static Formatting.Tests.Tester;

namespace Formatting.Tests.FormatsOn
{
    public class FormatOnPasteTests
    {
        private static void PasteTest(string original, PasteInfo pasteInfo, string expected)
        {
            string pasteFormatted = Tester.Format(pasteInfo.PasteString);
            string actual =
                original.Substring(0, pasteInfo.From) +
                pasteFormatted +
                original.Substring(pasteInfo.To, original.Length - pasteInfo.To);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Basic()
        {
            string original = "";
            PasteInfo paste = new PasteInfo("foo=function(x) end", 0, 0);
            string expected = "foo = function ( x ) end";
            FormatOnPasteTests.PasteTest(original, paste, expected);
        }
    }
}
