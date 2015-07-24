using System;

namespace Microsoft.VisualStudio.LuaLanguageService.Shared
{
    internal sealed class Constants
    {
        internal const string LanguageName = "Lua";
        internal const string FileExtension = ".lua";

        internal sealed class Guids
        {
            internal const string LanguageServiceString = "88A1F488-9D00-4896-A255-6F8251208B90";
            internal const string PackageString = "40c3d121-7e37-4d03-a9f8-f10bca9805f3";
            internal static readonly Guid Package = new Guid(PackageString);
            internal static readonly Guid LanguageService = new Guid(LanguageServiceString);
        }
    }
}
