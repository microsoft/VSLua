using Microsoft.VisualStudio.Composition;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.VisualStudio.Utilities;
using Xunit.Abstractions;

namespace Formatting.Tests
{
    public class WrappingTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public WrappingTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public void GeneralWrappingTest(string original, string expected)
        {
            string actual = Format(original);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EmptyFunction()
        {
            string original = "foo = function() end";
            string expected = @"foo = function()
end";
           GeneralWrappingTest(original, expected);
        }

        [Fact]
        public void OneReturn()
        {
            string original = "foo = function() return end";
            string expected =
                @"foo = function()
    return
end";
            GeneralWrappingTest(original, expected);
        }

        [Fact]
        public void OneReturnWithVariable()
        {
            string original = "foo = function() return x end";
            string expected = @"foo = function()\n    return x\nend";

            GeneralWrappingTest(original, expected);
        }

        private string Format(string original)
        {

            var factory = new EditorUtils.EditorHostFactory();
            var host = factory.CreateEditorHost();

            var buffer = host.CreateTextBuffer(original);
            var edit = buffer.CreateEdit();
            var applied = edit.Apply();

            return applied.GetText();
        }
    }
}
