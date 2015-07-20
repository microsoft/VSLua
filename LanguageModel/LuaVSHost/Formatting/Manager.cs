using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio;
using VSLua.Shared;
using OLECommandFlags = Microsoft.VisualStudio.OLE.Interop.OLECMDF;
using Microsoft.VisualStudio.Text.Editor;

using LanguageService.Formatting;
using LanguageModel;
using System.IO;

namespace VSLua.Formatting
{
    internal sealed class Manager : IMiniCommandFilter, IFormatter
    {
        private ITextBuffer textBuffer;
        private ITextView textView;
        private bool isClosed;
        private ITextSnapshot prePasteSnapshot;

        internal Manager(ITextBuffer textBuffer, ITextView textView)
        {
            // setup the undo history here...
            this.textBuffer = textBuffer;
            this.textView = textView;
        }

        public bool PreProcessCommand(Guid guidCmdGroup, uint commandId, IntPtr variantIn)
        {
            if (guidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)commandId)
                {
                    case VSConstants.VSStd2KCmdID.FORMATDOCUMENT:
                        {
                            this.FormatDocument();
                            return true;
                        }
                    case VSConstants.VSStd2KCmdID.FORMATSELECTION:
                        {
                            this.FormatSelection();
                            return true;
                        }
                }
            }
            else if (guidCmdGroup == typeof(VSConstants.VSStd97CmdID).GUID)
            {
                switch ((VSConstants.VSStd97CmdID)commandId)
                {
                    case VSConstants.VSStd97CmdID.Paste:
                        {
                            this.prePasteSnapshot = this.textView.TextSnapshot;
                        }
                        break;
                }
            }
            return false;
        }

        public void PostProcessCommand(Guid guidCmdGroup, uint commandId, IntPtr variantIn, bool wasHandled)
        {
            if (guidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)commandId)
                {
                    case VSConstants.VSStd2KCmdID.RETURN:
                        this.FormatOnEnter();
                        break;
                }
            }
            else if (guidCmdGroup == typeof(VSConstants.VSStd97CmdID).GUID)
            {
                switch ((VSConstants.VSStd97CmdID)commandId)
                {
                    case VSConstants.VSStd97CmdID.Paste:
                        {
                            this.FormatOnPaste();
                        }
                        break;
                }
            }


            // For typing stuff (after semicolin, } or enter and stuff)
        }

        public bool QueryCommandStatus(Guid guidCmdGroup, uint commandId, IntPtr commandText, out OLECMDF commandStatus)
        {
            commandStatus = new OLECommandFlags();

            if (guidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)commandId)
                {
                    case VSConstants.VSStd2KCmdID.FORMATDOCUMENT:
                        if (this.CanFormatDocument())
                        {
                            commandStatus = OLECommandFlags.OLECMDF_ENABLED | OLECommandFlags.OLECMDF_SUPPORTED;
                            return true;
                        }
                        break;

                    case VSConstants.VSStd2KCmdID.FORMATSELECTION:
                        if (this.CanFormatSelection())
                        {
                            commandStatus = OLECommandFlags.OLECMDF_SUPPORTED;
                            if (!this.textView.Selection.IsEmpty)
                            {
                                commandStatus |= OLECommandFlags.OLECMDF_ENABLED;
                            }
                            return true;
                        }
                        break;
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

        private bool CanFormatSelection()
        {
            return true;
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

        public void FormatOnEnter()
        {
            SnapshotPoint caret = this.textView.Caret.Position.BufferPosition;
            int lineNumber = caret.GetContainingLine().LineNumber;

            if (lineNumber > 0)
            {
                var snapshotLine = caret.Snapshot.GetLineFromLineNumber(lineNumber - 1);
                int startPos = snapshotLine.Start.Position;
                int endPos = caret.Position;
                SnapshotSpan span = new SnapshotSpan(this.textBuffer.CurrentSnapshot, Span.FromBounds(startPos, endPos));
                this.Format(span);
            }
        }

        public void FormatOnPaste()
        {
            INormalizedTextChangeCollection changes = this.prePasteSnapshot.Version.Changes;
            if (changes != null && changes.Count > 0)
            {
                ITextChange firstChange = changes[0];
                ITextChange lastChange = changes[changes.Count - 1];
                //int length = (lastChange.OldPosition + lastChange.OldLength) - firstChange.OldPosition;
                //SnapshotSpan oldSpan = EditorUtilities.CreateSnapshotSpan(this.prePasteSnapshot, firstChange.OldPosition, length);
                //SnapshotSpan newSpan = oldSpan.TranslateTo(this.textView.TextSnapshot, SpanTrackingMode.EdgeExclusive);

                SnapshotSpan newSpan = new SnapshotSpan(this.textView.TextSnapshot, Span.FromBounds(firstChange.NewPosition, lastChange.NewEnd));

                this.Format(newSpan);
            }
        }

        public void FormatSelection()
        {
            SnapshotSpan snapshotSpan = this.GetSelectionSpan();
            this.Format(snapshotSpan);
        }

        private SnapshotSpan GetSelectionSpan() // TODO: need meaningful format
        {
            int startPos = this.textView.Selection.Start.Position.Position;
            int endPos = this.textView.Selection.End.Position.Position;
            Span span = Span.FromBounds(startPos, endPos);
            SnapshotSpan snapshotSpan = new SnapshotSpan(this.textView.TextSnapshot, span);
            return snapshotSpan;
        }

        //public void FormatStatement()
        //{
        //    throw new NotImplementedException();
        //}

        private bool Format(SnapshotSpan span)
        {

            if (span.Snapshot.TextBuffer != this.textBuffer || span.IsEmpty || !this.CanFormatSpan(span))
            {
                return false;
            }

            SnapshotPoint startLinePoint = span.Start.GetContainingLine().Start;
            span = new SnapshotSpan(startLinePoint, span.End);
            ITextSnapshot textSnapshot = span.Snapshot;

            SourceText sourceText = SourceTextProvider.Get(textSnapshot);

            List<TextEditInfo> edits = Formatter.Format(sourceText, span.Start.Position, span.End.Position);

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
