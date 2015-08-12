/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

using System;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages.UserControls;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages
{
    [Guid(Guids.GeneralPageString)]
    internal class GeneralPage : BaseDialogPage
    {
        private GeneralUserControl generalUserControl;

        protected override UIElement Child
        {
            get
            {
                if (this.generalUserControl == null)
                {
                    this.generalUserControl = new GeneralUserControl();
                    this.generalUserControl.DataContext = UserSettings.MainInstance;
                }

                return this.generalUserControl;
            }
        }
    }
}
