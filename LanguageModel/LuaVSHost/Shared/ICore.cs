using System;
using Microsoft.VisualStudio.Editor;

namespace Microsoft.VisualStudio.LuaLanguageService.Shared
{
    internal interface ICore
    {
        GlobalEditorOptions GlobalEditorOptions { get; }
        IServiceProvider ServiceProvider { get; }
        IVsEditorAdaptersFactoryService EditorAdaptersFactory { get; }
    }
}
