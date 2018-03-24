// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests
{
    using Xunit;

    public class IndentationTests
    {

        private delegate void TestFunction(string original, string expected1);
        private TestFunction FormattingTest = Tester.FormattingTest;

        [Fact]
        public void BasicFunction()
        {
            string original = @"
foo = function()
return
end";
            string expected1 = @"
foo = function ()
    return
end";
            this.FormattingTest(original, expected1);
        }

        [Fact]
        public void BasicTable()
        {
            string original = @"
t1 = {
1,
}";
            string expected1 = @"
t1 = {
    1,
}";
            this.FormattingTest(original, expected1);
        }

        [Fact]
        public void EmbeddedFunctions()
        {
            string original = @"
foo = function ()
bar = function ()
end
end";
            string expected1 = @"
foo = function ()
    bar = function ()
    end
end";
            this.FormattingTest(original, expected1);
        }

        [Fact]
        public void EmbeddedTables()
        {
            string original = @"
t1 = {
t2 = {
x, y, z
},
t3 = {
x
}
}";
            string expected1 = @"
t1 = {
    t2 = {
        x, y, z
    },
    t3 = {
        x
    }
}";
            this.FormattingTest(original, expected1);
        }

        [Fact]
        public void AlreadyIndented1()
        {
            string original = @"
foo = function ()
    return
end";
            this.FormattingTest(original, original);
        }

        [Fact]
        public void FirstLineIndentedWrong()
        {
            string original = "     foo";
            string expected = "foo";
            this.FormattingTest(original, expected);
        }

        [Fact]
        public void OverIndentedFunction()
        {
            string original = @"
foo = function ()
                    return
end";
            string expected1 = @"
foo = function ()
    return
end";
            this.FormattingTest(original, expected1);
        }

        [Fact]
        public void SimpleOverIndent()
        {
            string original = @"t = {
        x";
            string expected1 = @"t = {
    x";
            this.FormattingTest(original, expected1);
        }

        [Fact]
        public void EndBug()
        {
            string original = @"
foo = function ()
return--comment
end";
            string expected1 = @"
foo = function ()
    return--comment
end";
            this.FormattingTest(original, expected1);
        }

        [Fact]
        public void Comment()
        {
            string original = @"
foo = function ()
--[[comment]] return
end";
            string expected1 = @"
foo = function ()
    --[[comment]] return
end";
            this.FormattingTest(original, expected1);
        }
    }
}
