using System;
using System.ComponentModel.Composition;
using LanguageService;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices.Lua.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Operations;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    [Export(typeof(ISingletons))]
    internal class Singletons : ISingletons
    {
        [Import]
        private IEditorOperationsFactoryService editorOperationsFactory;

        [Import]
        private IVsEditorAdaptersFactoryService editorAdaptersFactory;

        [Import]
        private SVsServiceProvider serviceProvider;

        private SourceTextCache sourceTextCache;
        private LuaFeatureContainer featureContainer;
        private Formatting.UserSettings userSettings;
        private IDocumentOperations documentOperations;

        public IVsEditorAdaptersFactoryService EditorAdaptersFactory
        {
            get
            {
                return this.editorAdaptersFactory;
            }
        }

        public IEditorOperationsFactoryService EditorOperationsFactory
        {
            get
            {
                return this.editorOperationsFactory;
            }
        }

        public IDocumentOperations DocumentOperations
        {
            get
            {
                if (this.documentOperations == null)
                {
                    this.documentOperations = new DocumentOperations(this);
                }

                return this.documentOperations;
            }

            internal set
            {
                // Used in unit-tests
                this.documentOperations = value;
            }
        }

        public SourceTextCache SourceTextCache
        {
            get
            {
                return this.sourceTextCache != null ? this.sourceTextCache : (this.sourceTextCache = new SourceTextCache());
            }
        }

        public LuaFeatureContainer FeatureContainer
        {
            get
            {
                if (this.featureContainer == null)
                {
                    this.featureContainer = new LuaFeatureContainer();
                }

                return this.featureContainer;
            }
        }

        public Formatting.UserSettings FormattingUserSettings
        {
            get
            {
                if (this.userSettings == null)
                {
                    var shell = this.serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
                    Assumes.Present(shell);
                    Guid guid = Guids.Package;
                    IVsPackage package;
                    ErrorHandler.ThrowOnFailure(shell.LoadPackage(ref guid, out package));
                    LuaLanguageServicePackage luaPackage = (LuaLanguageServicePackage)package;
                    this.userSettings = luaPackage.FormattingUserSettings;
                }

                return this.userSettings;
            }
        }

        public IServiceProvider ServiceProvider
        {
            get
            {
                return this.serviceProvider;
            }
        }
    }
}
