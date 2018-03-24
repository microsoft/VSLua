// Copyright (c) Microsoft. All rights reserved.

namespace Formatting.Tests
{
    using Xunit;

    public class NewlineRuleTesting
    {

        private delegate void TestFunction(string original, string expected1, string expected2);
        private TestFunction GeneralTest = Tester.FormattingTest;

        [Fact(Skip = "Not passing")]
        public void Basic1()
        {
            string original = @"
t1 = { 2,
}";
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

        [Fact(Skip = "Not Implemented")]
        public void Basic2()
        {
            string original = @"
t1 = {
}";
            this.GeneralTest(original, original, original);
        }

        [Fact(Skip = "Not passing")]
        public void Basic3()
        {
            string original = @"
t1 =
{}";
            string expected1 = @"
t1 =
{
}";
            this.GeneralTest(original, expected1, expected1);
        }

        [Fact(Skip = "Not passing")]
        public void Comment1()
        {
            string original = @"
t1 =-- comment
{}";
            string expected = @"
t1 =-- comment
{
}";
            this.GeneralTest(original, expected, expected);
        }

        [Fact(Skip = "Not passing")]
        public void Comment2()
        {
            string original = @"
t1 =
{--[[ comment ]]}";
            string expected = @"
t1 =
{
--[[ comment ]]}";
            this.GeneralTest(original, expected, expected);
        }

        [Fact(Skip = "Not passing")]
        public void Comment3()
        {
            string original = @"
t1 = {
    basic, --[[ comment ]]basic2}";
            string expected1 = @"
t1 = {
basic,
--[[ comment ]]basic2
}";
            string expected2 = @"
t1 = {
    basic,
    --[[ comment ]]basic2
    }";
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
        public void MultipleElements()
        {
            string original = @"
t1 = { 1, 2,
}";
            string expected1 = @"
t1 = {
1,
2,
}";
            string expected2 = @"
t1 = {
    1,
    2,
    }";
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
        public void ConstructorDifferentLine()
        {
            string original = @"
t1 =
{ 1, 2 }";
            string expected1 = @"
t1 =
{
    1,
    2,
}";
            string expected2 = @"
t1 =
{
1,
2,
}";
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
        public void EmbeddedTable()
        {
            string original = @"
t1 = {
t2 = { t3 = { } } }";
            string expected1 = @"
t1 = {
    t2 = { t3 = { } }
}";
            string expected2 = @"
t1 = {
    t2 = { t3 = { } }
    }";
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact(Skip = "Not passing")]
        public void EmbeddedTable2()
        {
            string original = @"
t1 = {
t2 = {
t3 = { } }
}";
            string expected1 = @"
t1 = {
    t2 = {
        t3 = { }
    }
}";
            string expected2 = @"
t1 = {
    t2 = {
        t3 = { }
        }
    }";
            this.GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void BrokenTable()
        {
            string original = "t = {";
            this.GeneralTest(original, original, original);
        }

        [Fact(Skip = "Not passing")]
        public void BrokenTable2()
        {
            string original = @"
t = {
1, 2, 3,";
            string expected1 = @"
t = {
1,
2,
3,";
            string expected2 = @"
t = {
    1,
    2,
    3,";
            this.GeneralTest(original, expected1, expected2);
        }
    }
}
