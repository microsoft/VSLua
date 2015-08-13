/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using LanguageService;
using LanguageService.Formatting;
using LanguageService.Formatting.Options;
using LanguageService.Shared;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

using OLECommandFlags = Microsoft.VisualStudio.OLE.Interop.OLECMDF;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting
{
    internal sealed class FormatCommandHandler : IMiniCommandFilter, IFormatter
    {
        internal FormatCommandHandler(ITextBuffer textBuffer, ITextView textView, ISingletons core)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));
            Requires.NotNull(textView, nameof(textView));
            Requires.NotNull(core, nameof(core));

            this.core = core;
            this.textBuffer = textBuffer;
            this.textView = textView;
        }

        private ITextBuffer textBuffer;
        private ITextView textView;
        private bool isClosed;
        private ISingletons core;
        private ITextSnapshot prePasteSnapshot;

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

            // For typing stuff (after semicolon, } or enter and stuff)
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
            if (this.core.FormattingUserSettings.FormatOnEnter != true)
            {
                return;
            }

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
            if (this.core.FormattingUserSettings.FormatOnPaste != true)
            {
                return;
            }

            SnapshotSpan? newSpan = EditorUtilities.GetPasteSpan(this.prePasteSnapshot, this.textView.TextSnapshot);
            if (newSpan != null)
            {
                this.Format((SnapshotSpan)newSpan);
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

        private bool Format(SnapshotSpan span)
        {
            if (span.Snapshot.TextBuffer != this.textBuffer || span.IsEmpty || !this.CanFormatSpan(span))
            {
                return false;
            }

            SnapshotPoint startLinePoint = span.Start.GetContainingLine().Start;
            span = new SnapshotSpan(startLinePoint, span.End);

            SourceText sourceText = this.core.SourceTextCache.Get(this.textBuffer.CurrentSnapshot);

            Range range = new Range(span.Start.Position, span.Length);

            FormattingOptions formattingOptions = this.GetFormattingOptions(this.core.FormattingUserSettings);

            List<TextEditInfo> edits = this.core.FeatureContainer.Formatter.Format(sourceText, range, formattingOptions);

            using (ITextEdit textEdit = this.textBuffer.CreateEdit())
            {
                foreach (TextEditInfo edit in edits)
                {
                    textEdit.Replace(edit.Start, edit.Length, edit.ReplacingWith);
                }

                textEdit.Apply();
            }

            return true;
        }

        private FormattingOptions GetFormattingOptions(UserSettings settings)
        {

            List<OptionalRuleGroup> disabledRuleGroups = this.GetDisabledRules(settings);

            FormattingOptions newOptions = new FormattingOptions(disabledRuleGroups, settings.TabSize, settings.IndentSize, settings.UsingTabs);

            return newOptions;
        }

        private List<OptionalRuleGroup> GetDisabledRules(UserSettings settings)
        {
            var disabledRules = new List<OptionalRuleGroup>();

            if (settings.AddSpacesOnInsideOfCurlyBraces != true)
            {
                disabledRules.Add(OptionalRuleGroup.SpaceOnInsideOfCurlyBraces);
            }

            if (settings.AddSpacesOnInsideOfParenthesis != true)
            {
                disabledRules.Add(OptionalRuleGroup.SpaceOnInsideOfParenthesis);
            }

            if (settings.AddSpacesOnInsideOfSquareBrackets != true)
            {
                disabledRules.Add(OptionalRuleGroup.SpaceOnInsideOfSquareBrackets);
            }

            if (settings.SpaceBetweenFunctionAndParenthesis != true)
            {
                disabledRules.Add(OptionalRuleGroup.SpaceBeforeOpenParenthesis);
            }

            if (settings.SpaceAfterCommas != true)
            {
                disabledRules.Add(OptionalRuleGroup.SpaceAfterCommas);
            }

            if (settings.SpaceBeforeAndAfterAssignmentInStatement != true)
            {
                disabledRules.Add(OptionalRuleGroup.SpaceBeforeAndAfterAssignmentForStatement);
            }

            if (settings.SpaceBeforeAndAfterAssignmentOperatorOnField != true)
            {
                disabledRules.Add(OptionalRuleGroup.SpaceBeforeAndAfterAssignmentForField);
            }

            if (settings.ForLoopAssignmentSpacing != true)
            {
                disabledRules.Add(OptionalRuleGroup.SpaceBeforeAndAfterAssignmentInForLoopHeader);
            }

            if (settings.ForLoopIndexSpacing != true)
            {
                disabledRules.Add(OptionalRuleGroup.NoSpaceBeforeAndAfterIndiciesInForLoopHeader);
            }

            if (settings.AddNewLinesToMultilineTableConstructors != true)
            {
                disabledRules.Add(OptionalRuleGroup.WrappingMoreLinesForTableConstructors);
            }

            if (settings.WrapSingleLineForLoops != true)
            {
                disabledRules.Add(OptionalRuleGroup.WrappingOneLineForFors);
            }

            if (settings.WrapSingleLineFunctions != true)
            {
                disabledRules.Add(OptionalRuleGroup.WrappingOneLineForFunctions);
            }

            if (settings.WrapSingleLineTableConstructors != true)
            {
                disabledRules.Add(OptionalRuleGroup.WrappingOneLineForTableConstructors);
            }

            if (settings.SpaceBeforeAndAfterBinaryOperations != true)
            {
                disabledRules.Add(OptionalRuleGroup.SpaceBeforeAndAfterBinaryOperations);
            }

            return disabledRules;
        }
    }
}
