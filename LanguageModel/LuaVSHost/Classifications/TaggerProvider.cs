using System.Collections.Generic;
using System.ComponentModel.Composition;
using LanguageService.Classification;
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

#pragma warning disable 0169, 0649

        [Import]
        private IStandardClassificationService standardClassifications;

        [Import]
        private ISingletons singletons;

#pragma warning restore 0169, 0649

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return this.singletons.Tagger as ITagger<T>;
        }

    }
}
