using LanguageService;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting;
using Microsoft.VisualStudio.LanguageServices.Lua.Text;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal interface ISingletons
    {
        IVsEditorAdaptersFactoryService EditorAdaptersFactory { get; }
        SourceTextCache SourceTextCache { get; }
        LuaFeatureContainer FeatureContainer { get; }
        UserSettings FormattingUserSettings { get; }
    }
}
