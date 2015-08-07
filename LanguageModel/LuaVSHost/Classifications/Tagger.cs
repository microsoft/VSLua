using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageService;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Classifications
{
    internal class Tagger : ITagger<ClassificationTag>
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private IStandardClassificationService standardClassifications;
        private ISingletons singletons;

        internal Tagger(IStandardClassificationService standardClassifications, ISingletons singletons)
        {
            Requires.NotNull(standardClassifications, nameof(standardClassifications));
            Requires.NotNull(singletons, nameof(singletons));

            this.standardClassifications = standardClassifications;
            this.singletons = singletons;
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (SnapshotSpan span in spans)
            {
                ITextSnapshot textSnapshot = span.Snapshot;
                SourceText sourceText = this.singletons.SourceTextCache.Get(textSnapshot);

                List<TagInfo>

                // I don't think ColorEdits is a great name, I just use it because I can't think of anything better right now.
                //
                // List<ColorEdit> colorEdits = this.core.FeatureContainer.Colorizer.Colorize(sourceText)
                //
                //foreach (ColorEdit colorEdit in colorEdits)
                //{
                //    ClassificationTag vsClassification = this.classifications[colorEdit.Classification];
                //    SnapshotSpan tokenSpan = new SnapshotSpan(textSnapshot, colorEdit.Start, colorEdit.Length);

                //    yield return TagSpan<ClassificationTag>(tokenSpan, vsClassification);
                //}

                yield break;
            }
        }
    }
}
