/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Classifications
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(Constants.Language.Name)]
    [TagType(typeof(ClassificationTag))]
    internal class TaggerProvider : ITaggerProvider
    {
        #pragma warning disable 0169, 0649 // Supress the "not" initialized warning
        [Import]
        private IStandardClassificationService standardClassifications;

        [Import]
        private IClassificationTypeRegistryService classificationTypeRegistry;

        [Import]
        private ISingletons singletons;
        #pragma warning restore 0169, 0649

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            Requires.NotNull(buffer, nameof(buffer));

            Func<Tagger> taggerCreator = () =>
            {
                Tagger tagger = new Tagger(buffer, this.standardClassifications, this.classificationTypeRegistry, this.singletons);

                return tagger;
            };

            // Taggers can be created for many reasons. In this case, it's fine to have a sinlge
            // tagger for a given buffer, so cache it on the buffer as a singleton property.
            return buffer.Properties.GetOrCreateSingletonProperty(taggerCreator) as ITagger<T>;
        }
    }
}
