// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages.UserControls;
using Microsoft.VisualStudio.LanguageServices.Lua.Shared;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages
{
    [Guid(Guids.WrappingAndNewLinePageString)]
    internal class WrappingAndNewLinePage : BaseDialogPage
    {
        private WrappingAndNewLineUserControl wrappingAndNewLineUserControl;

        protected override UIElement Child
        {
            get
            {
                if (this.wrappingAndNewLineUserControl == null)
                {
                    this.wrappingAndNewLineUserControl = new WrappingAndNewLineUserControl();
                    this.wrappingAndNewLineUserControl.DataContext = UserSettings.MainInstance;
                }

                return this.wrappingAndNewLineUserControl;
            }
        }
    }
}
