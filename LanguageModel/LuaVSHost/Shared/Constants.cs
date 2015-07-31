using System;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal sealed class Constants
    {
        internal sealed class Language
        {
            internal const string Name = "Lua";
            internal const string FileExtension = ".lua";
        }

        internal sealed class Formatting
        {
            internal const string Category = "Formatting";
            internal sealed class Pages
            {
                internal const string General = "General";
                internal const string Spacing = "Spacing";
                internal const string Indentation = "Indentation";
                internal const string WrappingAndNewLines = "WrappingNewLines";
                //internal const string NewLines = "NewLines";
            }
        }
    }
}
