using System.Collections.Generic;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Differencing;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting
{
    internal class TextView
    {
        private static Dictionary<IWpfTextView, TextView> viewMap = new Dictionary<IWpfTextView, TextView>();

        protected IWpfTextView WpfTextView { get; private set; }

        protected IVsTextView VsTextView { get; set; }

        protected ITextBuffer TextBuffer { get; set; }

        protected bool IsReadOnly { get; private set; }

        protected bool IsClosed { get; private set; }

        private IVsEditorAdaptersFactoryService editorAdaptersService;
        private ICore core;

        public TextView(IWpfTextView wpfTextView, ICore core)
        {
            this.core = core;
            this.editorAdaptersService = core.EditorAdaptersFactory;

            Validation.Requires.NotNull(wpfTextView, nameof(wpfTextView));

            this.WpfTextView = wpfTextView;
            this.VsTextView = this.editorAdaptersService.GetViewAdapter(this.WpfTextView);
            this.IsReadOnly = wpfTextView.Roles.Contains(DifferenceViewerRoles.LeftViewTextViewRole);

            viewMap.Add(this.WpfTextView, this);
        }

        internal void Connect(ITextBuffer textBuffer)
        {
            this.TextBuffer = textBuffer;
            CommandFilter filter = this.CreateCommandFilter(textBuffer);
        }

        protected CommandFilter CreateCommandFilter(ITextBuffer textBuffer)
        {
            CommandFilter filter = new CommandFilter();

            Manager formattingManager = new Manager(textBuffer, this.WpfTextView, this.core);
            filter.MiniFilters.Add(formattingManager);

            IOleCommandTarget nextFilter = null;

            this.VsTextView.AddCommandFilter(filter, out nextFilter);
            filter.Next = nextFilter;

            return filter;
        }
    }
}
