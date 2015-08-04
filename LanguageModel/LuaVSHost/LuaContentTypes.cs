using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLua.Shared;

namespace VSLua
{
    internal static class LuaContentTypes
    {
        [Export]
        [Name(Constants.LuaLanguageName)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition LuaContentType;

        [Export]
        [FileExtension(Constants.LuaFileExtension)]
        [ContentType(Constants.LuaLanguageName)]
        internal static FileExtensionToContentTypeDefinition LuaContentTypeExtensionDefintion;
    }
}
