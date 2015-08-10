namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal sealed class Constants
    {
        // Wait 2 seconds after last buffer change to actually update the UI
        // If changes come up during the way, cancel the current update task
        // and queue another.
        internal const int UIUpdateDelay = 1750;
        internal const int MaximumErrorsPerFile = 25;

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
