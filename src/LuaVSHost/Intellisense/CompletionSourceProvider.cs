using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Intellisense
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("lua")]
    [Name("token completion")]
    internal class CompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new CompletionSource(this, textBuffer);
        }
    }
}
