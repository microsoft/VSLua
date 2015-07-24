using System;

namespace Microsoft.VisualStudio.LuaLanguageService.Shared
{
    internal sealed class Constants
    {
        internal const string LuaLanguageName = "Lua";
        internal const string LuaFileExtension = ".lua";

        internal sealed class Guids
        {
            internal static readonly Guid LuaLanguageService = new Guid("88A1F488-9D00-4896-A255-6F8251208B90");
        }
    }
}
