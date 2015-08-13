using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Formatting.Tests.SmartIndent
{
    public class SmartIndentTests
    {
        [Fact]
        public void OnEnd()
        {
            string text = @"
    foo = function()
    end";
            Tester.SmartIndentationTest(text, lineNumber: 2, expectedIndent: 0);
        }

        [Fact]
        public void OnCloseBrace()
        {
            string text = @"
    t = {
    }";
            Tester.SmartIndentationTest(text, lineNumber: 2, expectedIndent: 0);
        }

        [Fact]
        public void Basic()
        {
            string text = @"foo = function
    ";

            Tester.SmartIndentationTest(text, lineNumber: 1, expectedIndent: 4);
        }

        [Fact]
        public void Basic2()
        {
            string text = "";

            Tester.SmartIndentationTest(text, lineNumber: 0, expectedIndent: 0);
        }

        [Fact]
        public void AfterFunction()
        {
            string text = @"
    foo = function()
    return
    end
    ";

            Tester.SmartIndentationTest(text, lineNumber: 4, expectedIndent: 0);
        }

        [Fact]
        public void BeforeFunction()
        {
            string text = @"
    foo = function()
    return
    end";

            Tester.SmartIndentationTest(text, lineNumber: 0, expectedIndent: 0);
        }

        [Fact]
        public void InFunction()
        {
            string text = @"
    foo function()

    return
    end";
            Tester.SmartIndentationTest(text, lineNumber: 2, expectedIndent: 4);
        }

        [Fact]
        public void BeforeTable()
        {
            string text = @"
    t = {
    1
    }";

            Tester.SmartIndentationTest(text, lineNumber: 0, expectedIndent: 0);
        }

        [Fact]
        public void AfterTable()
        {
            string text = @"
    t = {
    1
    }
    ";
            Tester.SmartIndentationTest(text, lineNumber: 4, expectedIndent: 0);
        }

        [Fact]
        public void InTable()
        {
            string text = @"
    t = {
    1,

    }";
            Tester.SmartIndentationTest(text, lineNumber: 3, expectedIndent: 4);
        }

        [Fact]
        public void UnfinishedTable()
        {
            string text = @"
    t = {
    ";
            Tester.SmartIndentationTest(text, lineNumber: 2, expectedIndent: 4);
        }

        [Fact]
        public void EmbeddedFunction()
        {
            string text = @"
    foo = function()
    bar = function()

    end
    end";
            Tester.SmartIndentationTest(text, lineNumber: 3, expectedIndent: 8);
        }

        [Fact]
        public void EmbeddedTable()
        {
            string text = @"
    t = {
    f = {

    }
    }";
            Tester.SmartIndentationTest(text, lineNumber: 3, expectedIndent: 8);
        }

        [Fact]
        public void EmbeddedUnfinishedTable()
        {
            string text = @"
    t = {
    f = {
    ";
            Tester.SmartIndentationTest(text, lineNumber: 3, expectedIndent: 8);
        }

        [Fact]
        public void EmbeddedUnfinishedFunction()
        {
            string text = @"
    foo = function()
    bar = function()
    ";
            Tester.SmartIndentationTest(text, lineNumber: 3, expectedIndent: 8);
        }

        [Fact]
        public void ThreeEmbeddedFunction()
        {
            string text = @"
    foo = function()
    bar = function()
        foobar = function()
    ";
            Tester.SmartIndentationTest(text, lineNumber: 4, expectedIndent: 12);
        }

        [Fact]
        public void MultipleLinesFunction()
        {
            string text = @"
    foo = function()


    bar = function()

    end";
            Tester.SmartIndentationTest(text, lineNumber: 5, expectedIndent: 8);

        }

        [Fact]
        public void SmartIndentBeforeAlreadyDefinedFunction()
        {
            string text = @"
    bar  = function ()

    end

    foo = function ()
    return x
    end";
            Tester.SmartIndentationTest(text, lineNumber: 2, expectedIndent: 4);
        }

        [Fact]
        public void SmartIndentBeforeAlreadyDefinedTable()
        {
            string text = @"
    t = {

    }

    t = {
    1
    }";
            Tester.SmartIndentationTest(text, lineNumber: 2, expectedIndent: 4);
        }


    }
}
