using System;
using System.Collections.Generic;
using LanguageModel;
using LanguageService.Formatting;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.LuaLanguageService.Shared;

using OLECommandFlags = Microsoft.VisualStudio.OLE.Interop.OLECMDF;

namespace Microsoft.VisualStudio.LuaLanguageService.Formatting
{
    internal sealed class Manager : IMiniCommandFilter, IFormatter
    {
        private ITextBuffer textBuffer;
        private ITextView textView;
        private bool isClosed;

        internal Manager(ITextBuffer textBuffer, ITextView textView)
        {
            // setup the undo history here...
            this.textBuffer = textBuffer;
            this.textView = textView;
        }

        public void PostProcessCommand(Guid guidCmdGroup, uint commandId, IntPtr variantIn, bool wasHandled)
        {
            // For typing stuff (after semicolin, } or enter and stuff)
        }

        public bool PreProcessCommand(Guid guidCmdGroup, uint commandId, IntPtr variantIn)
        {
            if (guidCmdGroup == VSConstants.VSStd2K)
            {
                if ((VSConstants.VSStd2KCmdID)commandId == VSConstants.VSStd2KCmdID.FORMATDOCUMENT)
                {
                    this.FormatDocument();
                    return true;
                }
            }
            return false;
        }

        public bool QueryCommandStatus(Guid guidCmdGroup, uint commandId, IntPtr commandText, out OLECMDF commandStatus)
        {
            commandStatus = new OLECommandFlags();

            if (guidCmdGroup == VSConstants.VSStd2K)
            {
                if ((VSConstants.VSStd2KCmdID)commandId == VSConstants.VSStd2KCmdID.FORMATDOCUMENT)
                {
                    if (this.CanFormatDocument())
                    {
                        commandStatus = OLECommandFlags.OLECMDF_ENABLED | OLECommandFlags.OLECMDF_SUPPORTED;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Close()
        {
            if (this.isClosed)
            {
                return;
            }

            // I would do stuff here when the FormattingCloses, which is when the textview also closes

            this.isClosed = true;
        }

        private bool CanFormatDocument()
        {
            int endPos = this.textBuffer.CurrentSnapshot.Length;
            return this.CanFormatSpan(new SnapshotSpan(this.textView.TextSnapshot, Span.FromBounds(0, endPos)));
        }

        private bool CanFormatSpan(SnapshotSpan span)
        {
            return !this.textBuffer.IsReadOnly(span);
        }


        public void FormatDocument()
        {
            int endPos = this.textBuffer.CurrentSnapshot.Length;
            SnapshotSpan span = new SnapshotSpan(this.textBuffer.CurrentSnapshot, Span.FromBounds(0, endPos));
            this.Format(span);
        }

        public void FormatOnEnter(SnapshotPoint caret)
        {
            throw new NotImplementedException();
        }

        public void FormatOnPaste()
        {
            throw new NotImplementedException();
        }

        public void FormatSelection()
        {
            throw new NotImplementedException();
        }

        public void FormatStatement()
        {
            throw new NotImplementedException();
        }

        private bool Format(SnapshotSpan span)
        {

            if (span.Snapshot.TextBuffer != this.textBuffer || span.IsEmpty || !this.CanFormatSpan(span))
            {
                return false;
            }

            SnapshotPoint startLinePoint = span.Start.GetContainingLine().Start;
            span = new SnapshotSpan(startLinePoint, span.End);

            SourceText sourceText = SourceTextProvider.Get(this.textBuffer.CurrentSnapshot);

            List<TextEditInfo> edits = Formatter.Format(sourceText);

            using (ITextEdit textEdit = this.textBuffer.CreateEdit())
            {
                foreach (TextEditInfo edit in edits)
                {
                    textEdit.Replace(edit.Start, edit.Length, edit.ReplacingString);
                }
                textEdit.Apply();
            }

            return true;

        }


    }
}
