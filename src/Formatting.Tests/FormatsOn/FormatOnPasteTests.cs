// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests.FormatsOn
{
    using Xunit;
    using static Formatting.Tests.Tester;

    public class FormatOnPasteTests
    {
        private static void PasteTest(string original, PasteInfo pasteInfo, string expected)
        {
            string actual = Tester.FormatOnPaste(original, pasteInfo);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Basic()
        {
            string original = "";
            PasteInfo paste = new PasteInfo("foo=function(x) end", 0, 0);
            string expected = "foo = function ( x ) end";
            PasteTest(original, paste, expected);
        }

        [Fact]
        public void EmptyPasteOnNonEmptyBuffer()
        {
            string original = "x = 10";
            PasteInfo paste = new PasteInfo("", 5, 0);
            string expected = "x = 10";
            PasteTest(original, paste, expected);
        }

        [Fact]
        public void EmptyPasteOnEmptyBuffer()
        {
            string original = "";
            PasteInfo paste = new PasteInfo("", 0, 0);
            string expected = "";
            PasteTest(original, paste, expected);
        }

        [Fact]
        public void PastingInLine()
        {
            string original = "foo=function";
            PasteInfo paste = new PasteInfo("(x,y,z) end", 12, 0);
            string expected = "foo=function( x, y, z ) end";
            PasteTest(original, paste, expected);
        }

        [Fact]
        public void PasteMultipleLines()
        {
            string original = @"foo=
";
            PasteInfo paste = new PasteInfo(@"x+1
x=y+1", 6, 0);
            string expected = @"foo=
x + 1
x = y + 1";
            PasteTest(original, paste, expected);
        }

        [Fact]
        public void PasteOverMultipleLines()
        {
            string original = @"x = 10
y = 30
z = x + y";
            PasteInfo paste = new PasteInfo("print(\"hello world\")", 0, original.Length);
            string expected = "print ( \"hello world\" )";
            PasteTest(original, paste, expected);
        }

        [Fact]
        public void PasteMultipleLinesOverMultipleLines()
        {
            string original = @"x = 10
y = 30
z=x+y";
            PasteInfo paste = new PasteInfo(@"x = x+y
y = y + x", 0, 14);
            string expected = @"x = x + y
y = y + x
z=x+y";
            PasteTest(original, paste, expected);
        }

        [Fact]
        public void PasteInBetweenLetters()
        {
            string original = "xy";
            PasteInfo paste = new PasteInfo("= x/y *", 1, 0);
            string expected = "x= x / y * y";
            PasteTest(original, paste, expected);
        }
    }
}
