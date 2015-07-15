using Xunit;

namespace Formatting.Tests
{
    public class TrailingWhitespaceTests
    {
        delegate void TestFunction(string original, string expected);
        TestFunction GeneralTest = Tester.GeneralTest;

        [Fact]
        public void Basic()
        {
            string original = "x = 10      ";
            string expected = "x = 10";
            GeneralTest(original, expected);
        }

        [Fact]
        public void LotsOfTrailing()
        {
            string original = @"
x
       
x       ";
            string expected = @"
x

x";
            GeneralTest(original, expected);
        }

        [Fact]
        public void Comments1()
        {
            string original = @"x = 10            -- hello world       ";
            GeneralTest(original, original);
        }

        [Fact]
        public void Comments2()
        {
            string original = "x = 10    --[[comment]]     ";
            string expected = "x = 10    --[[comment]]";
            GeneralTest(original, expected);
        }

        [Fact]
        public void MultipleLines()
        {
            string original = @"
x = 10           
-- comment here
--[[ block
comment 
here]]
x = x + 1            ";
            string expected = @"
x = 10
-- comment here
--[[ block
comment
here]]
x = x + 1";
            GeneralTest(original, expected);
        }

        [Fact]
        public void TrailingWhiteSpaceBeforeEof()
        {
            string original = @"x   
    ";
            string expected = @"x
";
            GeneralTest(original, expected);
        }


    }
}