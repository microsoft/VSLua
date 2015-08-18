using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Errors
{
    internal class ErrorListItem : ErrorTask
    {
        public SnapshotSpan ErrorSpan { get; }

        public ErrorListItem(SnapshotSpan errorSpan)
        {
            this.ErrorSpan = errorSpan;
        }
    }
}