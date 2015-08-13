using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal sealed class DocumentOperations : IDocumentOperations
    {
        private ISingletons singletons;

        internal DocumentOperations(ISingletons singletons)
        {
            this.singletons = singletons;
        }

        public bool OpenDocument(string path, out bool isAlreadyOpen, out IWpfTextView textView)
        {
            isAlreadyOpen = false;
            textView = null;

            IVsWindowFrame windowFrame;

            isAlreadyOpen = this.GetAlreadyOpenedDocument(path, out windowFrame);

            IVsTextView vsTextView = GetVsTextView(windowFrame);

            if (vsTextView == null)
            {
                // The file could be open in some non-text view (e.g.:  designer).
                return false;
            }

            textView = this.singletons.EditorAdaptersFactory.GetWpfTextView(vsTextView);

            return textView != null;
        }

        public bool GetAlreadyOpenedDocument(string path, out IVsWindowFrame windowFrame)
        {
            IVsUIHierarchy hierarchy;
            uint itemId;

            // DevDiv 248655:  Calling OpenFile on a previewed document will promote it.
            // Do not promote unnecessarily.  Simply activate the window if it exists.
            if (!VsShellUtilities.IsDocumentOpen(this.singletons.ServiceProvider, path, VSConstants.LOGVIEWID.Code_guid, out hierarchy, out itemId, out windowFrame) || windowFrame == null)
            {
                return false;
            }

            return ErrorHandler.Succeeded(windowFrame.Show());
        }

        public void NavigateTo(IWpfTextView textView, Span span, bool selectSpan, bool deferNavigationWithOutlining)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }

            Debug.Assert(span.End <= textView.TextSnapshot.Length, string.Format("span.End ({0}) > textView.TextSnapshot.Length ({1})", span.End, textView.TextSnapshot.Length));

            IEditorOperations editorOperations = this.singletons.EditorOperationsFactory.GetEditorOperations(textView);
            SnapshotSpan snapshotSpan = EditorUtilities.CreateSnapshotSpan(textView.TextSnapshot, span.Start, span.Length);
            VirtualSnapshotSpan virtualSnapshotSpan = new VirtualSnapshotSpan(snapshotSpan);
            Navigate(editorOperations, virtualSnapshotSpan, selectSpan);
        }

        private static void Navigate(IEditorOperations editorOperations, VirtualSnapshotSpan virtualSnapshotSpan, bool selectSpan)
        {
            VirtualSnapshotPoint selectionEnd = selectSpan ? virtualSnapshotSpan.End : virtualSnapshotSpan.Start;

            editorOperations.SelectAndMoveCaret(virtualSnapshotSpan.Start, selectionEnd, TextSelectionMode.Stream, EnsureSpanVisibleOptions.AlwaysCenter);
        }

        private static IVsTextView GetVsTextView(IVsWindowFrame windowFrame)
        {
            if (windowFrame == null)
            {
                throw new ArgumentNullException("windowFrame");
            }

            object docView;
            int hresult = windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out docView);

            if (ErrorHandler.Failed(hresult))
            {
                return null;
            }

            IVsTextView viewAdapter = docView as IVsTextView;

            if (viewAdapter != null)
            {
                return viewAdapter;
            }

            IVsCodeWindow codeWindow = docView as IVsCodeWindow;

            if (codeWindow != null)
            {
                IVsTextView codeView;

                if (ErrorHandler.Succeeded(codeWindow.GetPrimaryView(out codeView)) && codeView != null)
                {
                    return codeView;
                }
            }

            return null;
        }
    }
}
