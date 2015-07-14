using Xunit;

namespace Formatting.Tests
{
    public class IndentationTests
    {

        private void GeneralTest(string original, string expected1, string expected2)
        {
            string actual1 = Tester.Format(original);
            string actual2 = Tester.Format(original);
            Assert.Equal(expected1, actual1);
            Assert.Equal(expected2, actual2);
        }

        [Fact]
        public void BasicFunction()
        {
            string original = @"
foo = function
return
end";
            string expected1 = @"
foo = function
    return
end";
            string expected2 = @"
foo = function
      return
end";
            GeneralTest(original, expected1, expected2);
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
            string expected2 = @"
t1 = {
      1,
}";
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void EmbeddedFunctions()
        {
            string original = @"
foo = function
bar = function
end
end";
            string expected1 = @"
foo = function
    bar = function
    end
end";
            string expected2 = @"
foo = function
      bar = function
      end
end";
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void EmbeddedTables()
        {
            string original = @"
t1 = {
t2 = {
x, y, z
}
t3 = {
x
}
}";
            string expected1 = @"
t1 = {
    t2 = {
        x, y, z
    }
    t3 = {
        x
    }
}";
            string expected2 = @"
t1 = {
      t2 = {
            x, y, z
           }
      t3 = {
            x
           }
     }";
            GeneralTest(original, expected1, expected2);
        }

        [Fact]
        public void Comment()
        {
            string original = @"
foo = function
--[[comment]] return
end";
            string expected1 = @"
foo = function
    --[[comment]] return
end";
            string expected2 = @"
foo = function
      --[[comment]] return
end";
            GeneralTest(original, expected1, expected2);
        }
    }
}
