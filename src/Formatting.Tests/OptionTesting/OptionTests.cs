using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageService.Formatting.Options;
using Xunit;

namespace Formatting.Tests.OptionTesting
{
    public class OptionTests
    {

        private static void OptionsTest(string original, string expected, params DisableableRules[] disableableRules)
        {
            Tester.FormattingTest(original, expected, new FormattingOptions(new List<DisableableRules>(disableableRules), 4, 4, false));
        }

        [Fact]
        public void DisableCommaSpaces()
        {
            string original = "x  ,  y = 1  ,  3";
            string expected = "x,y = 1,3";
            OptionsTest(original, expected, DisableableRules.SpaceAfterCommas);
        }

        [Fact]
        public void DisableAssignmentInStatements()
        {
            string original = "x = { x = 1 } for i=3,5 do x = 2 end";
            string expected = "x={ x = 1 } for i = 3,5 do x=2 end";
            OptionsTest(original, expected, DisableableRules.SpaceBeforeAndAfterAssignmentForStatement);
        }

        [Fact]
        public void DisableAssignmentInFors()
        {
            string original = "for i  =  4,5 do x=1 end x = { x=1 }";
            string expected = "for i=4,5 do x = 1 end x = { x = 1 }";
            OptionsTest(original, expected, DisableableRules.SpaceBeforeAndAfterAssignmentInForLoopHeader);
        }

        [Fact]
        public void DisableAssignmentInFields()
        {
            string original = @"
t = {
    t = function () t = 10 t = { x = 5 } end,
    [t] = 10
}";
            string expected = @"
t = {
    t=function () t = 10 t = { x=5 } end,
    [ t ]=10
}";
            OptionsTest(original, expected, DisableableRules.SpaceBeforeAndAfterAssignmentForField);
        }

        [Fact]
        public void DisableSpaceBeforeParen()
        {
            string original = "foo = function   () end";
            string expected = "foo = function() end";
            OptionsTest(original, expected, DisableableRules.SpaceBeforeOpenParenthesis);
        }

        [Fact]
        public void DisableSpaceInsideOfParens()
        {
            string original = "foo(  x,y,z   )";
            string expected = "foo (x, y, z)";
            OptionsTest(original, expected, DisableableRules.SpaceOnInsideOfParenthesis);
        }

        [Fact]
        public void DisableSpaceInsideOfCurlys()
        {
            string original = "foo {  x, y, z   }";
            string expected = "foo {x, y, z}";
            OptionsTest(original, expected, DisableableRules.SpaceOnInsideOfCurlyBraces);
        }

        [Fact]
        public void DisableSpaceInsideOfSquare()
        {
            string original = "t = g[  1  ]";
            string expected = "t = g[1]";
            OptionsTest(original, expected, DisableableRules.SpaceOnInsideOfSquareBrackets);
        }

        [Fact]
        public void DisableBinaryOperationSpaces()
        {
            string original = "t = 1   +   4   / 5  * 4";
            string expected = "t = 1+4/5*4";
            OptionsTest(original, expected, DisableableRules.SpaceBeforeAndAfterBinaryOperations);
        }

        [Fact]
        public void DisableForLoopIndexSep()
        {
            string original = "for i = 10,  5 do end";
            string expected = "for i = 10, 5 do end";
            OptionsTest(original, expected, DisableableRules.NoSpaceBeforeAndAfterIndiciesInForLoopHeader);
        }

        [Fact(Skip = "Not implemented")]
        public void WrappingTestForFunctions()
        {
            string original = "foo = function () end";
            OptionsTest(original, original, DisableableRules.WrappingOneLineForFunctions);
        }

        [Fact(Skip = "not implemented")]
        public void WrappingTestForForLoops()
        {
            string original = "for i = 10,1 do end";
            OptionsTest(original, original, DisableableRules.WrappingOneLineForFors);
        }

        [Fact(Skip = "Not implemented")]
        public void WrappingTestForTables()
        {
            string original = "t = { 1, 3, 4 }";
            OptionsTest(original, original, DisableableRules.WrappingOneLineForTableConstructors);
        }

        [Fact(Skip = "not implemented")]
        public void NewlineTests()
        {
            string original = @"
t = { 1,
    2, 3 }";
            OptionsTest(original, original, DisableableRules.WrappingMoreLinesForTableConstructors);
        }
    }
}
