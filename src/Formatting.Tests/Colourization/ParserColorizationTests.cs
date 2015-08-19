using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageService;
using LanguageService.Classification;
using Xunit;


namespace Formatting.Tests.Colourization
{
    public class ParserColorizationTests
    {
        private static void ClassificationAndOrderTest(string text, Classification[] classifications, string[] tokenTexts)
        {
            var featureContainer = new LuaFeatureContainer();
            SourceText sourceText = new SourceText(text);

            List<Classification> actualClassifications = new List<Classification>();
            List<string> actualTokenTexts = new List<string>();

            foreach (TagInfo tagInfo in featureContainer.Colourizer.ColorizeParserTokens(sourceText))
            {
                actualClassifications.Add(tagInfo.Classification);
                actualTokenTexts.Add(text.Substring(tagInfo.Start, tagInfo.Length));
            }

            Assert.Equal(classifications, actualClassifications);
            Assert.Equal(tokenTexts, actualTokenTexts);
        }

        [Fact]
        public void BasicGlobals()
        {
            string text = "foo = 1";
            Classification[] expectedClassifications = new Classification[1] { Classification.Global };
            string[] expectedStrings = new string[1] { "foo" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact]
        public void BasicLocals()
        {
            string text = @"local x
x = 1";
            Classification[] expectedClassifications = new Classification[2] { Classification.Local, Classification.Local };
            string[] expectedStrings = new string[2] { "x", "x" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact]
        public void BasicParams()
        {
            string text = "function foo(x) x = 1 end";
            Classification[] expectedClassifications = new Classification[3] { Classification.Global, Classification.ParameterReference, Classification.ParameterReference };
            string[] expectedStrings = new string[3] { "foo", "x", "x" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact]
        public void BasicFields()
        {
            string text = "t = { hello = world, something, [here] = there }";
            Classification[] expectedClassifications = new Classification[6] { Classification.Global, Classification.Field, Classification.Global, Classification.Global, Classification.Global, Classification.Global };
            string[] expectedString = new string[6] { "t", "hello", "world", "something", "here", "there" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedString);
        }

        [Fact]
        public void GlobalsBeforeLocals()
        {
            string text = "x = 1 x = x + 1 local x x = 2";
            Classification[] expectedClassifications = new Classification[5] { Classification.Global, Classification.Global, Classification.Global, Classification.Local, Classification.Local };
            string[] expectedStrings = new string[5] { "x", "x", "x", "x", "x" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact]
        public void ParamsBeforeLocals()
        {
            string text = "foo = function(x) x = 2 local x x = 2 end";
            Classification[] expectedClassifications = new Classification[5] { Classification.Global, Classification.ParameterReference, Classification.ParameterReference, Classification.Local, Classification.Local };
            string[] expectedStrings = new string[5] { "foo", "x", "x", "x", "x" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact]
        public void LocalInFunction()
        {
            string text = "foo = function() local x x = 2 end x = 3";
            Classification[] expectedClassifications = new Classification[4] { Classification.Global, Classification.Local, Classification.Local, Classification.Global };
            string[] expectedStrings = new string[4] { "foo", "x", "x", "x" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact]
        public void FieldInFunction()
        {
            string text = @"
foo = function()
    t = {
        foo = bar
    }
    t.foo = rab
end";
            Classification[] expectedClassifications = new Classification[7] { Classification.Global, Classification.Global, Classification.Field, Classification.Global, Classification.Global,
            Classification.Field, Classification.Global };
            string[] expectedStrings = new string[7] { "foo", "t", "foo", "bar", "t", "foo", "rab" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact]
        public void EmbeddedParams()
        {
            string text = "foo = function(x) bar = function(y) z = x + y end end";
            Classification[] expectedClassifications = new Classification[7] { Classification.Global, Classification.ParameterReference, Classification.Global, Classification.ParameterReference,
            Classification.Global, Classification.ParameterReference, Classification.ParameterReference };
            string[] expectedStrings = new string[7] { "foo", "x", "bar", "y", "z", "x", "y" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact]
        public void EmbeddedFields()
        {
            string text = "t = { here = there, t = { something = Else } }";
            Classification[] expectedClassifications = new Classification[6] { Classification.Global, Classification.Field, Classification.Global, Classification.Field, Classification.Field,
            Classification.Global };
            string[] expectedStrings = new string[6] { "t", "here", "there", "t", "something", "Else" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact]
        public void TableWithGlobalsAndLocalsAndFields()
        {
            string text = "local x t = { x = x, y = x }";
            Classification[] expectedClassifications = new Classification[6] { Classification.Local, Classification.Global, Classification.Field, Classification.Local, Classification.Field, Classification.Local };
            string[] expectedStrings = new string[6] { "x", "t", "x", "x", "y", "x" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact]
        public void VariedTable()
        {
            string text = "t = { [here] = there, this = that, something }";
            Classification[] expectedClassifications = new Classification[6] { Classification.Global, Classification.Global, Classification.Global, Classification.Field, Classification.Global, Classification.Global };
            string[] expectedStrings = new string[6] { "t", "here", "there", "this", "that", "something" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }

        [Fact(Skip = "Parser Error")]
        public void ChainedFields()
        {
            string text = "x.foo:bar.something:Else()";
            Classification[] expectedClassifications = new Classification[5] { Classification.Global, Classification.Field, Classification.Field, Classification.Field, Classification.Field };
            string[] expectedStrings = new string[5] { "x", "foo", "bar", "something", "Else" };
            ClassificationAndOrderTest(text, expectedClassifications, expectedStrings);
        }
    }
}
