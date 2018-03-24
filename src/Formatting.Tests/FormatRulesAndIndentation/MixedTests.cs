// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests
{
    using Xunit;

    // Assumes all Rules are turned on, all with regular indentation...
    public class MixedTests
    {
        private delegate void TestFunction(string original, string expected1, string expected2);
        private TestFunction GeneralTest = Tester.FormattingTest;

        [Fact(Skip = "Not passing")]
        public void Basic()
        {
            string original = @"
x= 1
--this is an add function
add = function(x, y)
    return x+y;--adding
end


get_zero = function() return 0 end";
            string expected1 = @"
x = 1
-- this is an add function
add = function( x, y )
    return x + y
    ;-- adding
end


get_zero = function()
    return 0
end";
            string expected2 = @"
x = 1
-- this is an add function
add = function ( x, y )
      return x + y
      ; -- adding
end


get_zero = function()
           return 0
end";
            this.GeneralTest(original, expected1, expected2);
        }
    }
}
