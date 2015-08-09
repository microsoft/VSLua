using System;
using LanguageService;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting;
using Microsoft.VisualStudio.LanguageServices.Lua.Text;
using Microsoft.VisualStudio.Text.Operations;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    internal interface ISingletons
    {
        IDocumentOperations DocumentOperations { get; }
        IEditorOperationsFactoryService EditorOperationsFactory { get; }
        IVsEditorAdaptersFactoryService EditorAdaptersFactory { get; }
        LuaFeatureContainer FeatureContainer { get; }
        UserSettings FormattingUserSettings { get; }
        IServiceProvider ServiceProvider { get; }
        SourceTextCache SourceTextCache { get; }
    }
}
