using Xunit;

namespace Formatting.Tests
{
    public class WrappingTests
    {

        delegate void TestFunction(string original, string expected1, string expected2);
        TestFunction GeneralTest = Tester.GeneralTest;

        [Fact]
        public void EmptyFunction()
        {
            string original = @"
    foo = function() end";
            string expected = @"
    foo = function()
    end";
            GeneralTest(original, expected, expected);
        }

        [Fact]
        public void Comment1()
        {
            string original = @"
    foo = function() --[[ comment ]]end";
            string expected = @"
    foo = function()
    --[[ comment ]]end";
            GeneralTest(original, expected, expected);
        }

        [Fact]
        public void Comment2()
        {
            string original = @"
    foo = function () --[[ comment ]]return end";
            string expected1 = @"
    foo = function()
    --[[ comment ]]return
    end";
            string expected2 = @"
    foo = function()
        --[[ comment ]]return
    end";
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void Comment3()
        {
            string original = @"
    t1 = {--[[ comment ]]}";
            string expected1 = @"
    t1 = {
    --[[ comment ]]}";
            string expected2 = @"
    t1 = {
        --[[ comment ]]}";
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void Comment4()
        {
            string original = @"
    t1 = { --[[ comment ]]basic}";
            string expected1 = @"
    t1 = {
    --[[ comment ]] basic
    }";
            string expected2 = @"
    t1 = {
        --[[ comment ]] basic
        }";
            GeneralTest(original, expected1, expected2);
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
            GeneralTest(original, expected1, expected2);
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

            GeneralTest(original, expected1, expected2);
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
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void EmptyFunctionV2()
        {
            string original = @"
    function foo() end";
            string expected = @"
    function foo()
    end";
            GeneralTest(original, expected, expected);
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
            GeneralTest(original, expected, expected);
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
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void NotSameLineFunction1()
        {
            string original = @"
    foo = function() return
    end";
            GeneralTest(original, original, original);
        }

        [Fact]
        public void NotSameLineFunction2()
        {
            string original = @"
    foo =
    function() return end";
            GeneralTest(original, original, original);
        }

        [Fact]
        public void NotSameLineFunction3()
        {
            string original = @"
    function foo()
    end";
            GeneralTest(original, original, original);
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
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void OneElement()
        {
            string original = @"
    t1 = {2}";
            string expected1 = @"
    t1 = {
    2
    }";
            string expected2 = @"
    t1 = {
        2
        }";

            GeneralTest(original, expected1, expected2);

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
            GeneralTest(original, expected1, expected2);
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
            GeneralTest(original, expected1, expected2);

        }

        [Fact]
        public void TableMultipleLines1()
        {
            string original = @"
    t1 =
    {}";
            GeneralTest(original, original, original);
        }

        [Fact]
        public void TableMutlipleLines2()
        {
            string original = @"
    t2 = {2,
    }";
            GeneralTest(original, original, original);
        }

        [Fact]
        public void TableMutlipleLines3()
        {
            string original = @"
    t3 = {2
    ,3}";
            GeneralTest(original, original, original);
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

            GeneralTest(original, expected1, expected2);
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
            GeneralTest(original, expected1, expected2);
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
            GeneralTest(original, expected, expected);
        }
    }
}
