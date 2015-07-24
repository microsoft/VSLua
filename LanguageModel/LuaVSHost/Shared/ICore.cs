using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LuaLanguageService.Formatting;

namespace Microsoft.VisualStudio.LuaLanguageService.Shared
{
    internal interface ICore
    {
        GlobalEditorOptions GlobalEditorOptions { get; }
        IServiceProvider ServiceProvider { get; }
        IVsEditorAdaptersFactoryService EditorAdaptersFactory { get; }
    }
}
