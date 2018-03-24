// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests
{
    using Xunit;

    public class WrappingTests
    {

        private delegate void TestFunction(string original, string expected1, string expected2);
        private TestFunction GeneralTest = Tester.FormattingTest;

        [Fact(Skip = "Not passing")]
        public void EmptyFunction()
        {
            string original = @"
foo = function() end";
            string expected = @"
foo = function()
end";
            this.GeneralTest(original, expected, expected);
        }

        [Fact(Skip = "Not passing")]
        public void Comment1()
        {
            string original = @"
    foo = function() --[[ comment ]]end";
            string expected = @"
    foo = function()
    --[[ comment ]]end";
            this.GeneralTest(original, expected, expected);
        }

        [Fact(Skip = "Not passing")]
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
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
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
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
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
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not Implemented")]
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
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
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

            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not Implemented")]
        public void TwoReturns()
        {
            string original = @"
foo = function() x = 10 return end";
            string expected1 = @"
foo = function()
    x = 10
    return
end";
            string expected2 = @"
foo = function()
    x = 10
    return
end";
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip ="Not Implemented")]
        public void EmptyFunctionV2()
        {
            string original = @"
function foo() end";
            string expected = @"
function foo()
end";
            this.GeneralTest(original, expected, expected);
        }

        [Fact(Skip = "Not passing")]
        public void OneReturnV2()
        {
            string original = @"
    function foo() return end";
            string expected = @"
    function foo()
    return
    end";
            this.GeneralTest(original, expected, expected);
        }

        [Fact(Skip = "Not passing")]
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
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
        public void NotSameLineFunction1()
        {
            string original = @"
    foo = function() return
    end";
            this.GeneralTest(original, original, original);
        }

        [Fact(Skip = "Not passing")]
        public void NotSameLineFunction2()
        {
            string original = @"
    foo =
    function() return end";
            this.GeneralTest(original, original, original);
        }

        [Fact(Skip = "Not passing")]
        public void NotSameLineFunction3()
        {
            string original = @"
    function foo()
    end";
            this.GeneralTest(original, original, original);
        }

        [Fact(Skip = "Not passing")]
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
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
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

            this.GeneralTest(original, expected1, expected2);

        }

        [Fact(Skip = "Not passing")]
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
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
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
            this.GeneralTest(original, expected1, expected2);

        }

        [Fact(Skip = "Not implemented")]
        public void TableMultipleLines1()
        {
            string original = @"
t1 =
{}";
            this.GeneralTest(original, original, original);
        }

        [Fact(Skip = "Not passing")]
        public void TableMutlipleLines2()
        {
            string original = @"
t2 = {2,
}";
            this.GeneralTest(original, original, original);
        }

        [Fact(Skip = "Not passing")]
        public void TableMutlipleLines3()
        {
            string original = @"
t3 = {2
,3}";
            this.GeneralTest(original, original, original);
        }

        [Fact(Skip = "Not passing")]
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

            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
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
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
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
            this.GeneralTest(original, expected, expected);
        }

        [Fact(Skip = "not implemented")]
        public void ForWrapping1()
        {
            string original = "for i = 10,3 do end";
            string expected = @"for i = 10,3 do
end";
            this.GeneralTest(original, expected, expected);
        }

        [Fact(Skip = "not implemented")]
        public void ForWrapping2()
        {
            string original = "for i = 10,3 do x = 10 y = 4 z = x + y end";
            string expected = @"for i = 10,3 do
    x = 10
    y = 4
    z = x + y
end";
            this.GeneralTest(original, expected, expected);
        }

    }
}
