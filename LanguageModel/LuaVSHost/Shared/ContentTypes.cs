using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal static class ContentTypes
    {
        [Export]
        [Name(Constants.Language.Name)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition ContentType { get; set; }

        [Export]
        [FileExtension(Constants.Language.FileExtension)]
        [ContentType(Constants.Language.Name)]
        internal static FileExtensionToContentTypeDefinition LuaContentTypeExtensionDefinition { get; set; }
    }
}
