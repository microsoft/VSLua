using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Differencing;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudio.LuaLanguageService.Formatting
{
    internal class TextView
    {
        private static Dictionary<IWpfTextView, TextView> viewMap = new Dictionary<IWpfTextView, TextView>();



        protected IWpfTextView WpfTextView { get; private set; }
        protected IVsTextView VsTextView { get; set; }
        protected ITextBuffer TextBuffer { get; set; }
        protected bool IsReadOnly { get; private set; }
        protected bool IsClosed { get; private set; }

        protected IVsEditorAdaptersFactoryService editorAdaptersService;

        public TextView(IWpfTextView wpfTextView, IVsEditorAdaptersFactoryService editorAdaptersService)
        {

            this.editorAdaptersService = editorAdaptersService;

            if (wpfTextView == null)
            {
                throw new ArgumentNullException("wpfTextView");
            }

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

            Manager formattingManager = new Manager(textBuffer, this.WpfTextView);
            filter.MiniFilters.Add(formattingManager);

            IOleCommandTarget nextFilter = null;

            this.VsTextView.AddCommandFilter(filter, out nextFilter);
            filter.Next = nextFilter;

            return filter;

        }
    }
}
