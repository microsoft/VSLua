using System;
using System.ComponentModel.Composition;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Errors
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(ErrorTag))]
    [ContentType(Constants.Language.Name)]
    internal sealed class ErrorTaggerProvider : ITaggerProvider
    {
        [Import]
        private ISingletons singletons = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            Validate.IsNotNull(buffer, nameof(buffer));

            Func<ErrorTagger> errorTaggerCreator = () =>
            {
                ErrorTagger tagger = new ErrorTagger(buffer, this.singletons);

                return tagger;
            };

            // Taggers can be created for many reasons. In this case, it's fine to have a sinlge
            // tagger for a given buffer, so cache it on the buffer as a singleton property.
            return buffer.Properties.GetOrCreateSingletonProperty<ErrorTagger>(errorTaggerCreator) as ITagger<T>;
        }
    }
}
