// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages.UserControls;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages
{
    [Guid(Guids.SpacingPageString)]
    internal class SpacingPage : BaseDialogPage
    {
        private SpacingUserControl spacingUserControl;

        protected override UIElement Child
        {
            get
            {
                if (this.spacingUserControl == null)
                {
                    this.spacingUserControl = new SpacingUserControl();
                    this.spacingUserControl.DataContext = UserSettings.MainInstance;
                }

                return this.spacingUserControl;
            }
        }
    }
}
