using System;
using System.Collections.Generic;
using LanguageService;
using LanguageService.Classification;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Classifications
{
    internal class Tagger : ITagger<ClassificationTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private IStandardClassificationService standardClassifications;
        private ISingletons singletons;
        private Dictionary<Classification, IClassificationType> vsClassifications;

        internal Tagger(IStandardClassificationService standardClassifications, ISingletons singletons)
        {
            Requires.NotNull(standardClassifications, nameof(standardClassifications));
            Requires.NotNull(singletons, nameof(singletons));

            this.standardClassifications = standardClassifications;
            this.singletons = singletons;
            this.vsClassifications = this.InitializeDictionary(standardClassifications);
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (SnapshotSpan span in spans)
            {
                ITextSnapshot textSnapshot = span.Snapshot;
                SourceText sourceText = this.singletons.SourceTextCache.Get(textSnapshot);

                foreach (TagInfo tagInfo in this.singletons.FeatureContainer.Colourizer.Colourize(sourceText))
                {
                    SnapshotSpan tokenSpan = new SnapshotSpan(textSnapshot, tagInfo.Start, tagInfo.Length);
                    IClassificationType classification = this.standardClassifications.Other;
                    this.vsClassifications.TryGetValue(tagInfo.Classification, out classification);

                    yield return new TagSpan<ClassificationTag>(tokenSpan, new ClassificationTag(classification));
                }
            }
        }

        private Dictionary<Classification, IClassificationType> InitializeDictionary(IStandardClassificationService standardClassifications)
        {
            var something = standardClassifications.Comment;
            return new Dictionary<Classification, IClassificationType>()
            {
                { Classification.Comment, standardClassifications.Comment },
                { Classification.Identifier, standardClassifications.Identifier },
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
