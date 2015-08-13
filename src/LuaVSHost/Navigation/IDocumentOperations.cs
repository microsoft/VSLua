using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal interface IDocumentOperations
    {
        bool OpenDocument(string path, out bool isAlreadyOpen, out IWpfTextView textView);
        void NavigateTo(IWpfTextView textView, Span span, bool selectSpan, bool deferNavigationWithOutlining);
        bool GetAlreadyOpenedDocument(string path, out IVsWindowFrame windowFrame);
    }
}
