/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageService;
using LanguageService.Classification;
using LanguageService.Shared;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Classifications
{
    internal class Tagger : DisposableObject, ITagger<ClassificationTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private IStandardClassificationService standardClassifications;
        private ISingletons singletons;
        private Dictionary<Classification, IClassificationType> vsClassifications;
        private ITextBuffer buffer;
        private CancellationTokenSource cancellationTokenSource;
        private bool doParserRelatedColorization;

        internal Tagger(ITextBuffer buffer, IStandardClassificationService standardClassifications, ISingletons singletons)
        {
            Requires.NotNull(standardClassifications, nameof(standardClassifications));
            Requires.NotNull(singletons, nameof(singletons));

            this.buffer = buffer;
            this.singletons = singletons;

            this.standardClassifications = standardClassifications;
            this.singletons = singletons;
            this.vsClassifications = this.InitializeDictionary(standardClassifications);

            this.buffer.Changed += this.OnBufferChanged;
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (!this.doParserRelatedColorization)
            {
                return this.GetTagsFromLexer(spans);
            }
            else
            {
                return this.GetTagsFromParser(spans);
            }
        }

        private IEnumerable<ITagSpan<ClassificationTag>> GetTagsFromParser(NormalizedSnapshotSpanCollection spans)
        {
            // spans should be only one span... the entire visible buffer I suspect - I'll find out later. TODO
            if (spans.Count <= 0)
            {
                yield break;
            }

            var span = spans[0];
            var snapshot = span.Snapshot;

            SourceText sourceText = this.singletons.SourceTextCache.Get(snapshot);

            foreach (TagInfo tagInfo in this.singletons.FeatureContainer.Colourizer.ColorizeParserTokens(sourceText))
            {
                SnapshotSpan tokenSpan = new SnapshotSpan(snapshot, tagInfo.Start, tagInfo.Length);
                IClassificationType classification = this.standardClassifications.Other;
                this.vsClassifications.TryGetValue(tagInfo.Classification, out classification);

                yield return new TagSpan<ClassificationTag>(tokenSpan, new ClassificationTag(classification));
            }
        }

        private IEnumerable<ITagSpan<ClassificationTag>> GetTagsFromLexer(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count < 0)
            {
                yield break;
            }

            List<Range> ranges = new List<Range>();

            foreach (SnapshotSpan span in spans)
            {
                ranges.Add(new Range(span.Start.Position, span.Length));
            }

            ITextSnapshot snapshot = this.buffer.CurrentSnapshot;
            SourceText sourceText = this.singletons.SourceTextCache.Get(snapshot);

            foreach (TagInfo tagInfo in this.singletons.FeatureContainer.Colourizer.ColorizeLexerTokens(sourceText, ranges))
            {
                SnapshotSpan tokenSpan = new SnapshotSpan(snapshot, tagInfo.Start, tagInfo.Length);
                IClassificationType classification = this.standardClassifications.Other;
                this.vsClassifications.TryGetValue(tagInfo.Classification, out classification);

                yield return new TagSpan<ClassificationTag>(tokenSpan, new ClassificationTag(classification));
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
            this.UpdateParserRelatedClassifications(e.After, this.cancellationTokenSource.Token);
        }

        private void UpdateParserRelatedClassifications(ITextSnapshot snapshot, CancellationToken token)
        {
            Task.Run(async () =>
            {
                await Task.Delay(Constants.UIUpdateDelay);

                if (token.IsCancellationRequested)
                {
                    return;
                }

                this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
            }, token);

            this.doParserRelatedColorization = true;
        }

        private Dictionary<Classification, IClassificationType> InitializeDictionary(IStandardClassificationService standardClassifications)
        {
            var something = standardClassifications.Comment;
            return new Dictionary<Classification, IClassificationType>()
            {
                { Classification.Comment, standardClassifications.Comment },
                { Classification.Keyword, standardClassifications.Keyword },
                { Classification.KeyValue, standardClassifications.NumberLiteral },
                { Classification.Operator, standardClassifications.Operator },
                { Classification.Number, standardClassifications.NumberLiteral },
                { Classification.Punctuation, standardClassifications.Other },
                { Classification.StringLiteral, standardClassifications.StringLiteral },
                { Classification.Bracket, standardClassifications.SymbolDefinition }
            };
        }
    }
}
