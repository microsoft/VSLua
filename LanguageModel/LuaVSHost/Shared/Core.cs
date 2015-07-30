using System;
using System.ComponentModel.Composition;
using LanguageService;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.LuaLanguageService.Formatting;

namespace Microsoft.VisualStudio.LuaLanguageService.Shared
{
    [Export(typeof(ICore))]
    internal class Core : ICore
    {
        [Import]
        private GlobalEditorOptions globalEditorOptions;

        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider serviceProvider;

        [Import]
        private IVsEditorAdaptersFactoryService editorAdaptersFactory;

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

        public IServiceProvider ServiceProvider
        {
            get
            {
                return this.serviceProvider;
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
                    userSettings = new UserSettings();
                }
                return userSettings;
            }
        }
    }
}
