using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices.Lua.Errors;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Differencing;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Text
{
    internal class TextView
    {
        private IVsEditorAdaptersFactoryService editorAdaptersService;
        private ISingletons core;
        private ErrorListPresenter errorListPresenter;
        private CommandFilter filter;

        protected IWpfTextView WpfTextView { get; }

        protected IVsTextView VsTextView { get; set; }

        protected ITextBuffer TextBuffer { get; set; }

        protected bool IsReadOnly { get; }

        public ErrorListPresenter ErrorListPresenter
        {
            get
            {
                return this.errorListPresenter;
            }
            set
            {
                this.errorListPresenter = value;
                this.errorListPresenter.UpdateErrorList(this.TextBuffer.CurrentSnapshot);
            }
        }

        public TextView(IWpfTextView wpfTextView, ISingletons core)
        {
            Requires.NotNull(wpfTextView, nameof(wpfTextView));
            Requires.NotNull(core, nameof(core));

            this.core = core;
            this.editorAdaptersService = core.EditorAdaptersFactory;

            this.WpfTextView = wpfTextView;
            this.VsTextView = this.editorAdaptersService.GetViewAdapter(this.WpfTextView);
            this.IsReadOnly = wpfTextView.Roles.Contains(DifferenceViewerRoles.LeftViewTextViewRole);
        }

        internal void Connect(ITextBuffer textBuffer)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));

            this.TextBuffer = textBuffer;
            this.filter = this.CreateCommandFilter(textBuffer);
            this.ErrorListPresenter = new ErrorListPresenter(this.WpfTextView, this.core);
        }

        internal void Disconnect(ITextBuffer textBuffer)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));

            this.filter?.Close();

            this.errorListPresenter?.Dispose();
            this.errorListPresenter = null;
        }

        protected CommandFilter CreateCommandFilter(ITextBuffer textBuffer)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));

            CommandFilter filter = new CommandFilter();

            FormatCommandHandler formattingManager = new FormatCommandHandler(textBuffer, this.WpfTextView, this.core);
            filter.MiniFilters.Add(formattingManager);

            IOleCommandTarget nextFilter = null;

            this.VsTextView.AddCommandFilter(filter, out nextFilter);
            filter.Next = nextFilter;

            return filter;
        }
    }
}
