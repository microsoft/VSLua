using Xunit;

namespace Formatting.Tests
{
    public class NewlineRuleTesting
    {

        delegate void TestFunction(string original, string expected1, string expected2);
        TestFunction GeneralTest = Tester.GeneralTest;

        [Fact]
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
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void Basic2()
        {
            string original = @"
t1 = {
}";
            GeneralTest(original, original, original);
        }

        [Fact]
        public void Basic3()
        {
            string original = @"
t1 =
{}";
            string expected1 = @"
t1 =
{
}";
            GeneralTest(original, expected1, expected1);
        }

        [Fact]
        public void Comment1()
        {
            string original = @"
t1 =-- comment
{}";
            string expected = @"
t1 =-- comment
{
}";
            GeneralTest(original, expected, expected);
        }

        [Fact]
        public void Comment2()
        {
            string original = @"
t1 =
{--[[ comment ]]}";
            string expected = @"
t1 =
{
--[[ comment ]]}";
        }

        [Fact]
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
            GeneralTest(original, expected1, expected2);
        }
        
        [Fact]
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
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
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
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
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
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
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
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void BrokenTable()
        {
            string original = "t = {";
            GeneralTest(original, original, original);
        }

        [Fact]
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
            GeneralTest(original, expected1, expected2);
        }







    }
}
