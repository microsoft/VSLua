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

        public async Task GeneralWrappingTest(string original, string expected)
        {
            string actual = await FormatAsync(original);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EmptyFunction()
        {
            string original = "foo = function() end";
            string expected = @"foo = function()
end";
           await GeneralWrappingTest(original, expected);
        }

        [Fact]
        public async Task OneReturn()
        {
            string original = "foo = function() return end";
            string expected =
                @"foo = function()
    return
end";
            await GeneralWrappingTest(original, expected);
        }

        [Fact]
        public async Task OneReturnWithVariable()
        {
            string original = "foo = function() return x end";
            string expected = @"foo = function()\n    return x\nend";

            await GeneralWrappingTest(original, expected);
        }

        private async Task<string> FormatAsync(string original)
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
