using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Formatting.Tests
{
    public class IndentSpacesTests
    {
        private static string testStringOneIndent = @"f = function ()
    return
    end";

        private static string testStringTwoIndents = @"f = function ()
    g = function ()
    return
    end
    end";

        private static void GeneralOneIndent(uint tabSize, uint indentSize, int expectedTabs, int expectedSpaces, bool usingTabs)
        {
            Tester.GeneralIndentAmountTest(testStringOneIndent,
                tabSize, indentSize, expectedTabs, expectedSpaces, usingTabs);
        }

        private static void GeneralTwoIndents(uint tabSize, uint indentSize, int expectedTabs, int expectedSpaces, bool usingTabs)
        {
            Tester.GeneralIndentAmountTest(testStringTwoIndents,
                tabSize, indentSize, expectedTabs, expectedSpaces, usingTabs);
        }

        [Fact]
        public void BasicSpaces()
        {
            GeneralOneIndent(tabSize: 4, indentSize: 4, expectedTabs: 0, expectedSpaces: 4, usingTabs: false);
        }

        [Fact]
        public void BasicSpaces2()
        {
            GeneralTwoIndents(tabSize: 4, indentSize: 4, expectedTabs: 0, expectedSpaces: 8, usingTabs: false);
        }

        [Fact]
        public void BasicTabs()
        {
            GeneralOneIndent(tabSize: 4, indentSize: 4, expectedTabs: 1, expectedSpaces: 0, usingTabs: true);
        }

        [Fact]
        public void BasicTabs2()
        {
            GeneralTwoIndents(tabSize: 4, indentSize: 4, expectedTabs: 2, expectedSpaces: 0, usingTabs: true);
        }

        [Fact]
        public void MoreTabThanIndentSpaces()
        {
            GeneralOneIndent(tabSize: 8, indentSize: 4, expectedTabs: 0, expectedSpaces: 4, usingTabs: false);
        }

        [Fact]
        public void MoreIndentThanTabSpaces()
        {
            GeneralOneIndent(tabSize: 4, indentSize: 8, expectedTabs: 0, expectedSpaces: 8, usingTabs: false);
        }

        [Fact]
        public void MoreTabThanIndentTabs()
        {
            GeneralOneIndent(tabSize: 8, indentSize: 4, expectedTabs: 0, expectedSpaces: 4, usingTabs: true);
        }

        [Fact]
        public void MoreIndentThanTabTabs()
        {
            GeneralOneIndent(tabSize: 4, indentSize: 8, expectedTabs: 2, expectedSpaces: 0, usingTabs: true);
        }

        [Fact]
        public void MoreTabThanIndentSpaces2()
        {
            GeneralTwoIndents(tabSize: 8, indentSize: 4, expectedTabs: 0, expectedSpaces: 8, usingTabs: false);
        }

        [Fact]
        public void MoreIndentThanTabSpaces2()
        {
            GeneralTwoIndents(tabSize: 4, indentSize: 8, expectedTabs: 0, expectedSpaces: 16, usingTabs: false);
        }

        [Fact]
        public void MoreTabThanIndentTabs2()
        {
            GeneralTwoIndents(tabSize: 8, indentSize: 4, expectedTabs: 1, expectedSpaces: 0, usingTabs: true);
        }

        [Fact]
        public void MoreIndentThanTabTabs2()
        {
            GeneralTwoIndents(tabSize: 4, indentSize: 8, expectedTabs: 4, expectedSpaces: 0, usingTabs: true);
        }

        [Fact]
        public void WeirdSizesSpaces()
        {
            GeneralOneIndent(tabSize: 3, indentSize: 5, expectedTabs: 0, expectedSpaces: 5, usingTabs: false);
        }

        [Fact]
        public void WeirdSizesSpaces2()
        {
            GeneralTwoIndents(tabSize: 3, indentSize: 5, expectedTabs: 0, expectedSpaces: 10, usingTabs: false);
        }

        [Fact]
        public void WeirdSizesTabs()
        {
            GeneralOneIndent(tabSize: 3, indentSize: 5, expectedTabs: 1, expectedSpaces: 2, usingTabs: true);
        }

        [Fact]
        public void WeirdSizesTabs2()
        {
            GeneralTwoIndents(tabSize: 3, indentSize: 5, expectedTabs: 3, expectedSpaces: 1, usingTabs: true);
        }

        [Fact]
        public void WeirdSizesSpacesB()
        {
            GeneralOneIndent(tabSize: 5, indentSize: 3, expectedTabs: 0, expectedSpaces: 3, usingTabs: false);
        }

        [Fact]
        public void WeirdSizesSpaces2B()
        {
            GeneralTwoIndents(tabSize: 5, indentSize: 3, expectedTabs: 0, expectedSpaces: 6, usingTabs: false);
        }

        [Fact]
        public void WeirdSizesTabsB()
        {
            GeneralOneIndent(tabSize: 5, indentSize: 3, expectedTabs: 0, expectedSpaces: 3, usingTabs: true);
        }

        [Fact]
        public void WeirdSizesTabs2B()
        {
            GeneralTwoIndents(tabSize: 5, indentSize: 3, expectedTabs: 1, expectedSpaces: 1, usingTabs: true);
        }
    }
}
