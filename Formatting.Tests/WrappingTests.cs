using Xunit;

namespace Formatting.Tests
{
    public class WrappingTests
    {

        public void GeneralWrappingTest(string original, string expected1, string expected2)
        {
            string actual = Tester.Format(original);
            Assert.Equal(expected1, actual);
            Assert.Equal(expected2, actual);
        }

        [Fact]
        public void EmptyFunction()
        {
            string original = @"
foo = function() end";
            string expected = @"
foo = function()
end";
           GeneralWrappingTest(original, expected, expected);
        }

        [Fact]
        public void OneReturn()
        {
            string original = @"
foo = function() return end";
            string expected1 = @"
foo = function()
    return
end";
            string expected2 = @"
foo = function()
      return
end";
            GeneralWrappingTest(original, expected1, expected2);
        }

        [Fact]
        public void OneReturnWithVariable()
        {
            string original = @"
foo = function() return x end";
            string expected1 = @"
foo = function()
    return x
end";
            string expected2 = @"
foo = function()
      return x
end";

            GeneralWrappingTest(original, expected1, expected2);
        }

        [Fact]
        public void TwoReturns()
        {
            string original = @"
foo = function() return return end";
            string expected1 =@"
foo = function()
    return
    return
end";
            string expected2 = @"
foo = function()
      return
      return
end";
            GeneralWrappingTest(original, expected1, expected2);
        }

        [Fact]
        public void EmptyFunctionV2()
        {
            string original = @"
function foo() end";
            string expected = @"
function foo()
end";
            GeneralWrappingTest(original, expected, expected);
        }

        [Fact]
        public void OneReturnV2()
        {
            string original = @"
function foo() return end";
            string expected = @"
function foo()
    return
end";
            GeneralWrappingTest(original, expected, expected);
        }

        [Fact]
        public void OneExpression()
        {
            string original = @"
foo = function() x = x + 1 end";
            string expected1 = @"
foo = function()
    x = x + 1
end";
            string expected2 = @"
foo = function()
      x = x + 1
end";
            GeneralWrappingTest(original, expected1, expected2);
        }

        [Fact]
        public void NotSameLineFunction1()
        {
            string original = @"
foo = function() return
end";
            GeneralWrappingTest(original, original, original);
        }

        [Fact]
        public void NotSameLineFunction2()
        {
            string original = @"
foo =
function() return end";
            GeneralWrappingTest(original, original, original);
        }

        [Fact]
        public void NotSameLineFunction3()
        {
            string original = @"
function foo()
end";
            GeneralWrappingTest(original, original, original);
        }

        [Fact]
        public void EmptyTable()
        {
            string original = @"
t1 = {}";
            string expected1 = @"
t1 = {
}";
            string expected2 = @"
t1 = {
     }";
            GeneralWrappingTest(original, expected1, expected2);
        }

        [Fact]
        public void OneElement()
        {
            string original = @"
t1 = {2}";
            string expected1 =@"
t1 = {
    2
}";
            string expected2 =@"
t1 = {
      2
     }";

            GeneralWrappingTest(original, expected1, expected2);

        }

        [Fact]
        public void TableTrailingComma()
        {
            string original = @"
t1 = {2,}";
            string expected1 = @"
t1 = {
    2,
}";
            string expected2 = @"
t1 = {
      2,
     }";
            GeneralWrappingTest(original, expected1, expected2);
        }

        [Fact]
        public void TableMoreElements()
        {
            string original = @"
t1 = {1, 2, 3}";
            string expected1 = @"
t1 = {
    1,
    2,
    3,
}";
            string expected2 = @"
t1 = {
      1,
      2,
      3,
     }";
            GeneralWrappingTest(original, expected1, expected2);

        }

        [Fact]
        public void TableMultipleLines1()
        {
            string original = @"
t1 =
{}";
            GeneralWrappingTest(original, original, original);
        }

        [Fact]
        public void TableMutlipleLines2()
        {
            string original = @"
t2 = {2,
}";
            GeneralWrappingTest(original, original, original);
        }

        [Fact]
        public void TableMutlipleLines3()
        {
            string original = @"
t3 = {2
,3}";
            GeneralWrappingTest(original, original, original);
        }

        [Fact]
        public void EmbeddedTables()
        {
            string original = @"
t1 = { t2 = { t3 = {} } }";
            string expected1 = @"
t1 = {
    t2 = {
        t3 = {
        }
    }
}";
            string expected2 = @"
t1 = {
      t2 = {
            t3 = {
                 }
           }
      }";

            GeneralWrappingTest(original, expected1, expected2);
        }

        [Fact]
        public void EmbeddedFunctions()
        {
            string original = @"
foo = function() bar = function() foobar = function() end end end";
            string expected1 = @"
foo = function()
    bar = function()
        foobar = function()
        end
    end
end";
            string expected2 = @"
foo = function()
      bar = function()
            foobar = function()
            end
      end
end";
            GeneralWrappingTest(original, expected1, expected2);
        }

        [Fact]
        public void EmbeddedFunctionsV2()
        {
            string original = @"
function foo() function bar() function foobar() end end end";
            string expected = @"
function foo()
    function bar()
        function foobar()
        end
    end
end";
            GeneralWrappingTest(original, expected, expected);
        }
    }
}
