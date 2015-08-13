using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Classifications
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(Constants.Language.Name)]
    [TagType(typeof(ClassificationTag))]
    internal class TaggerProvider : ITaggerProvider
    {
        [Import]
        private IStandardClassificationService standardClassifications;

        [Import]
        private ISingletons singletons;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return this.singletons.Tagger as ITagger<T>;
        }

    }
}
