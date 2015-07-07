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

        public async Task OneReturn()
        {
            string original = "foo = function() return end";
            string expected =
                @"foo = function()
    return
end";
            await GeneralWrappingTest(original, expected);
        }

        private async Task<string> FormatAsync(string original)
        {
            var ep = await EditorHost.CreateExportProviderAsync(this.testOutputHelper);
            var editorFacotryService = ep.GetExportedValue<ITextEditorFactoryService>();
            var textView = editorFacotryService.CreateTextView();
            var textBuffer = textView.TextBuffer;
            var textEdit = textBuffer.CreateEdit();
            textEdit.Replace(0, 0, original);
            var applied = textEdit.Apply();

            //List<TextEditInfo> textEdits = Formatter.Format(original);

            return applied.GetText();
        }
    }
}
