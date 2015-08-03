using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Shared
{
    [Export(typeof(GlobalEditorOptions))]
    internal sealed class GlobalEditorOptions : IVsTextManagerEvents2
    {
        internal vsIndentStyle IndentStyle { get; private set; }
        internal uint TabSize { get; private set; }
        private ConnectionPointCookie connectionPoint;

        internal event EventHandler<EventArgs> OnUpdateLanguagePreferences;

        [ImportingConstructor]
        internal GlobalEditorOptions(SVsServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsTextManager2 textManager = (IVsTextManager2)serviceProvider.GetService(typeof(SVsTextManager));
            VIEWPREFERENCES2[] viewPreferences = new VIEWPREFERENCES2[] { new VIEWPREFERENCES2() };
            LANGPREFERENCES2[] languagePreferences = new LANGPREFERENCES2[] { new LANGPREFERENCES2() { guidLang = Guids.Service } };

            int hresult = textManager.GetUserPreferences2(viewPreferences, pFramePrefs: null, pLangPrefs: languagePreferences, pColorPrefs: null);
            ErrorHandler.ThrowOnFailure(hresult);

            this.UpdatePreferences(viewPreferences, languagePreferences);

            this.connectionPoint = new ConnectionPointCookie(textManager, this, typeof(IVsTextManagerEvents2), true);
        }

        private void UpdatePreferences(VIEWPREFERENCES2[] viewPreferences, LANGPREFERENCES2[] languagePreferences)
        {
            if (viewPreferences != null && viewPreferences.Length > 0)
            {
                // this.AutoDelimiterHighlight = Convert.ToBoolean(viewPreferences[0].fAutoDelimiterHighlight);
            }

            if (languagePreferences != null && languagePreferences.Length > 0 &&
                Guid.Equals(languagePreferences[0].guidLang, Guids.Service))
            {
                this.IndentStyle = languagePreferences[0].IndentStyle;
                this.TabSize = languagePreferences[0].uTabSize;
                this.FireOnUpdateLanguagePreferences();
            }
        }

        private void FireOnUpdateLanguagePreferences()
        {
            this.OnUpdateLanguagePreferences?.Invoke(this, EventArgs.Empty);
        }

        public int OnRegisterMarkerType(int iMarkerType)
        {
            return VSConstants.S_OK;
        }

        public int OnRegisterView(IVsTextView pView)
        {
            return VSConstants.S_OK;
        }

        public int OnUnregisterView(IVsTextView pView)
        {
            return VSConstants.S_OK;
        }

        public int OnUserPreferencesChanged2(VIEWPREFERENCES2[] viewPreferences, FRAMEPREFERENCES2[] pFramePrefs, LANGPREFERENCES2[] languagePreferences, FONTCOLORPREFERENCES2[] pColorPrefs)
        {
            this.UpdatePreferences(viewPreferences, languagePreferences);
            return VSConstants.S_OK;
        }

        public int OnReplaceAllInFilesBegin()
        {
            return VSConstants.S_OK;
        }

        public int OnReplaceAllInFilesEnd()
        {
            return VSConstants.S_OK;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}
