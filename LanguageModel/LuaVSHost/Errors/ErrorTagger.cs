using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageService;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Errors
{
    internal sealed class ErrorTagger : DisposableObject, ITagger<ErrorTag>
    {
        // Wait 2 seconds after last buffer change to actually update the UI
        // If changes come up during the way, cancel the current update task
        // and queue another.
        private const int UIUpdateDelay = 2000;

        private ITextBuffer buffer;
        private CancellationTokenSource cancellationTokenSource;
        private ISingletons singletons;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public ErrorTagger(ITextBuffer buffer, ISingletons singletons)
        {
            Validate.IsNotNull(buffer, nameof(buffer));
            Validate.IsNotNull(singletons, nameof(singletons));

            this.buffer = buffer;
            this.singletons = singletons;

            this.buffer.Changed += this.OnBufferChanged;
        }

        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            // It is possible that we could be asked for tags for any text buffer,
            // not necessarily the one text buffer that this tagger knows about.
            if (spans[0].Snapshot.TextBuffer != this.buffer)
            {
                yield break;
            }

            ITextSnapshot textSnapshot = spans[0].Snapshot.TextBuffer.CurrentSnapshot;
            SourceText sourceText = this.singletons.SourceTextCache.Get(textSnapshot);
            IReadOnlyList<ParseError> errors = this.singletons.FeatureContainer.DiagnosticsProvider.GetDiagnostics(sourceText);

            if (errors.Count == 0)
            {
                yield break;
            }

            SnapshotSpan spanOfEntireSpansCollection = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(textSnapshot, SpanTrackingMode.EdgeExclusive);
            int entireEndIncludingLineBreak = spanOfEntireSpansCollection.End.GetContainingLine().EndIncludingLineBreak;

            for (int errorIndex = 0; errorIndex < errors.Count; errorIndex++)
            {
                ParseError error = errors[errorIndex];

                if (error.Start > entireEndIncludingLineBreak)
                {
                    continue;
                }

                ITextSnapshotLine line = spanOfEntireSpansCollection.Snapshot.GetLineFromPosition(error.Start);
                int errorStart = (error.Start <= line.End || error.Start == line.EndIncludingLineBreak) ? error.Start : line.End;

                // Determine whether error intersects requested span
                if (((errorStart + error.Length) < spanOfEntireSpansCollection.Start) || (errorStart > spanOfEntireSpansCollection.End))
                {
                    continue;
                }

                SnapshotSpan newSnapshotSpan = EditorUtilities.CreateSnapshotSpan(textSnapshot, errorStart, error.Length);

                yield return new TagSpan<ErrorTag>(newSnapshotSpan, new ErrorTag(PredefinedErrorTypeNames.SyntaxError, error.Message));
            }
        }

        protected override void DisposeManagedResources()
        {
            if (this.buffer != null)
            {
                this.buffer.Changed -= this.OnBufferChanged;
            }

            base.DisposeManagedResources();
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

        private void UpdateErrorsWithDelay(ITextSnapshot snapshot, CancellationToken token)
        {
            Task.Run(async () =>
            {
                await Task.Delay(ErrorTagger.UIUpdateDelay);

                if (token.IsCancellationRequested)
                {
                    return;
                }

                this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
            }, token);
        }
    }
}
