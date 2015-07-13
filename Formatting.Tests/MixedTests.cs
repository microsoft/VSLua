using Xunit;

namespace Formatting.Tests
{
    // Assumes all Rules are turned on, all with regular indentation...
    public class MixedTests
    {
        public void GeneralTest(string original, string expected1, string expected2)
        {
            string actual = Tester.Format(original);
            Assert.Equal(expected1, actual);
            Assert.Equal(expected2, actual);
        }

        [Fact]
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
            GeneralTest(original, expected1, expected2);
        }
    }
}
