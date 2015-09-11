using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Intellisense
{
    [Export(typeof(IVsTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [ContentType("lua")]
    internal class CompletionHandlerProvider : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService = null;

        [Import]
        internal ICompletionBroker CompletionBroker { get; set; }

        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = this.AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
            {
                return;
            }

            Func<CompletionCommandHandler> createCommandHandler = delegate  { return new CompletionCommandHandler(textViewAdapter, textView, this); };
            textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
        }
    }
}
