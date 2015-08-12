using System;
using System.ComponentModel.Composition;
using LanguageService;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;

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

        public GlobalEditorOptions GlobalEditorOptions => this.globalEditorOptions;

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
    }
}
