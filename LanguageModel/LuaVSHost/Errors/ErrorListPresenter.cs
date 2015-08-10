using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using LanguageService;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Constants = Microsoft.VisualStudio.LanguageServices.Lua.Shared.Constants;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Errors
{
    internal sealed class ErrorListPresenter : DisposableObject
    {
        private CancellationTokenSource cancellationTokenSource;
        private ErrorListProvider errorListProvider;
        private ISingletons singletons;
        private IWpfTextView textView;

        public ErrorListPresenter(IWpfTextView textView, ISingletons singletons)
        {
            Validate.IsNotNull(textView, nameof(textView));
            Validate.IsNotNull(singletons, nameof(singletons));

            this.textView = textView;
            this.singletons = singletons;
            this.errorListProvider = new ErrorListProvider(singletons.ServiceProvider);
            this.errorListProvider.ProviderGuid = Guid.NewGuid();
            this.errorListProvider.ProviderName = Constants.Language.Name;

            this.textView.TextBuffer.Changed += this.OnBufferChanged;
        }

        private void ClearErrors()
        {
            foreach (object task in this.errorListProvider.Tasks)
            {
                ErrorListItem errorTask = task as ErrorListItem;

                if (errorTask != null)
                {
                    errorTask.Navigate -= this.OnErrorListItemNavigate;
                }
            }

            this.errorListProvider.Tasks.Clear();
        }

        private ErrorListItem CreateErrorListItem(SnapshotSpan snapshot, ParseError error, string filePath)
        {
            ITextSnapshotLine line = snapshot.Snapshot.GetLineFromPosition(snapshot.Start);

            var errorListItem = new ErrorListItem(snapshot);
            errorListItem.Category = TaskCategory.All;
            errorListItem.Priority = TaskPriority.Normal;
            errorListItem.Document = filePath;
            errorListItem.ErrorCategory = TaskErrorCategory.Error;
            errorListItem.Text = error.Message;
            errorListItem.Line = line.LineNumber;
            errorListItem.Column = Math.Min(line.End, snapshot.Start) - line.Start;

            errorListItem.Navigate += this.OnErrorListItemNavigate;

            return errorListItem;
        }

        protected override void DisposeManagedResources()
        {
            //is this implementation correct? Do I just need to call this method.
            if (this.textView.TextBuffer != null)
            {
                this.textView.TextBuffer.Changed -= this.OnBufferChanged;
            }

            this.ClearErrors();

            this.errorListProvider.Dispose();
            this.errorListProvider = null;

            base.DisposeManagedResources();
        }

        private void OnErrorListItemNavigate(object sender, EventArgs e)
        {
            var errorListItem = sender as ErrorListItem;

            if (errorListItem != null)
            {
                IDocumentOperations docOperations = this.singletons.DocumentOperations;
                IWpfTextView targetTextView;
                bool isAlreadyOpen;

                if (!docOperations.OpenDocument(errorListItem.Document, out isAlreadyOpen, out targetTextView))
                {
                    return;
                }

                Debug.Assert(isAlreadyOpen, "We do not show errors for closed files.  Somehow we opened a closed file with errors in the error list.");

                docOperations.NavigateTo(targetTextView, errorListItem.ErrorSpan.Span, selectSpan: true, deferNavigationWithOutlining: false);
            }
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
            }

            this.cancellationTokenSource = new CancellationTokenSource();
            this.UpdateErrorsWithDelay(e.After, this.cancellationTokenSource.Token);
        }

        internal void UpdateErrorList(ITextSnapshot snapshot)
        {
            if (this.errorListProvider == null)
            {
                return;
            }

            try
            {
                this.errorListProvider.SuspendRefresh();
                this.ClearErrors();

                SourceText sourceText = this.singletons.SourceTextCache.Get(snapshot);
                IReadOnlyList<ParseError> errors = this.singletons.FeatureContainer.DiagnosticsProvider.GetDiagnostics(sourceText);

                if (errors.Count == 0)
                {
                    return;
                }

                int errorCount = 0;

                for (int i = 0; i < errors.Count; ++i)
                {
                    ParseError error = errors[i];

                    if (errorCount >= Constants.MaximumErrorsPerFile)
                    {
                        break;
                    }

                    SnapshotSpan errorSnapshotSpan = EditorUtilities.CreateSnapshotSpan(snapshot, error.Start, error.Length);

                    string filePath = snapshot.TextBuffer.GetFilePath();
                    Debug.Assert(filePath != null, "We should always be able to get the moniker for a file opened in the editor (even if it hasn't been saved)");

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        ErrorListItem errorListItem = this.CreateErrorListItem(errorSnapshotSpan, error, filePath);

                        this.errorListProvider.Tasks.Add(errorListItem);
                    }

                    errorCount++;
                }
            }
            finally
            {
                this.errorListProvider.ResumeRefresh();
            }
        }

        private void UpdateErrorsWithDelay(ITextSnapshot snapshot, CancellationToken token)
        {
            Task.Run(async () =>
            {
                await Task.Delay(Constants.UIUpdateDelay);

                if (token.IsCancellationRequested)
                {
                    return;
                }

                this.UpdateErrorList(snapshot);
            }, token);
        }
    }
}
