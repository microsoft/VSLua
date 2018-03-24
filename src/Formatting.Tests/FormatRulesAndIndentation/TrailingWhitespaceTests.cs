// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests
{
    using Xunit;

    public class TrailingWhitespaceTests
    {
        private delegate void TestFunction(string original, string expected);
        private TestFunction GeneralTest = Tester.FormattingTest;

        [Fact]
        public void Basic()
        {
            string original = "x = 10      ";
            string expected = "x = 10";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void LotsOfTrailing()
        {
            string original = @"
    x = 10
       
    x = 10       ";
            string expected = @"
x = 10

x = 10";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void Comments1()
        {
            string original = @"x = 10            -- hello world       ";
            this.GeneralTest(original, original);
        }

        [Fact]
        public void Comments2()
        {
            string original = "x = 10    --[[comment]]     ";
            string expected = "x = 10    --[[comment]]";
            this.GeneralTest(original, expected);
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
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void TrailingWhiteSpaceBeforeEof()
        {
            string original = @"x = 10   
    ";
            string expected = @"x = 10
";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void FunctionTrailing()
        {
            string original = @"
foo = function ()
     
end";
            string expected = @"
foo = function ()

end";
            this.GeneralTest(original, expected);
        }

        [Fact]
        public void EmbeddedFunctionTrailing()
        {
            string original = @"
foo = function ()
    t = {
    
    }
end";
            string expected = @"
foo = function ()
    t = {

    }
end";
            this.GeneralTest(original, expected);
        }


    }
}