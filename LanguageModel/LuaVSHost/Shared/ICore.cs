using System;
using LanguageService;
using Microsoft.VisualStudio.Editor;

namespace Microsoft.VisualStudio.LuaLanguageService.Shared
{
    internal interface ICore
    {
        GlobalEditorOptions GlobalEditorOptions { get; }
        IServiceProvider ServiceProvider { get; }
        IVsEditorAdaptersFactoryService EditorAdaptersFactory { get; }
        SourceTextCache SourceTextCache { get; }
        LuaFeatureContainer FeatureContainer { get; }
    }
}
