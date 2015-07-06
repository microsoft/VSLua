using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Formatting.Tests
{
    public class Class1
    {
        [Fact]
        public void Addition()
        {
            Assert.Equal("ab", concat("a", "b"));
        }

        private static string concat(string a, string b)
        {
            return a;
        }
    }
}
