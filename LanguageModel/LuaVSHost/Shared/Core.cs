using System;
using System.ComponentModel.Composition;
using LanguageService;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.LuaLanguageService.Formatting;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    [Export(typeof(ICore))]
    internal class Core : ICore
    {
        [Import]
        private GlobalEditorOptions globalEditorOptions;

        [Import]
        private IVsEditorAdaptersFactoryService editorAdaptersFactory;

        [Import]
        private SVsServiceProvider serviceProvider;

        private SourceTextCache sourceTextCache;
        private LuaFeatureContainer featureContainer;
        private UserSettings userSettings;

        public GlobalEditorOptions GlobalEditorOptions
        {
            get
            {
                return this.globalEditorOptions;
            }
        }

        public IVsEditorAdaptersFactoryService EditorAdaptersFactory
        {
            get
            {
                return this.editorAdaptersFactory;
            }
        }

        public SourceTextCache SourceTextCache
        {
            get
            {
                return sourceTextCache != null ? sourceTextCache : (sourceTextCache = new SourceTextCache());
            }
        }

        public LuaFeatureContainer FeatureContainer
        {
            get
            {
                if (featureContainer == null)
                {
                    featureContainer = new LuaFeatureContainer();
                }
                return featureContainer;
            }
        }

        public UserSettings UserSettings
        {
            get
            {
                if (userSettings == null)
                {
                    var shell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
                    Assumes.Present(shell);
                    Guid guid = Guids.Package;
                    IVsPackage package;
                    ErrorHandler.ThrowOnFailure(shell.LoadPackage(ref guid, out package));
                    LuaLanguageServicePackage luaPackage = (LuaLanguageServicePackage)package;
                    userSettings = luaPackage.FormattingUserSettings;
                }
                return userSettings;
            }
        }
    }
}
