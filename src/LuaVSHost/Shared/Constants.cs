using System.Windows.Media;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal sealed class Constants
    {
        // Wait UIUpdateDelay milliseconds after last buffer change to actually update the UI
        // If changes come up during the way, cancel the current update task
        // and queue another.
        internal const int UIUpdateDelay = 875;
        internal const int MaximumErrorsPerFile = 25;

        internal sealed class Language
        {
            internal const string Name = "Lua";
            internal const string FileExtension = ".lua";
        }

        internal sealed class Colourization
        {
            internal const int ParserUpdateDelay = 440;

            internal const string GlobalName = "LuaGlobalClassificationFormat";
            internal const string GlobalDisplayName = "Lua Global Identifier";

            internal const string LocalName = "LuaLocalClassificationFormat";
            internal const string LocalDisplayName = "Lua Local Identifier";

            internal const string ParamName = "LuaParamClassificationFormat";
            internal const string ParamDisplayName = "Lua Parameter Reference";

            internal const string FieldName = "LuaFieldClassificationFormat";
            internal const string FieldDisplayName = "Lua Field";
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
            }
        }
    }
}
