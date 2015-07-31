using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
                if (wrappingAndNewLineUserControl == null)
                {
                    wrappingAndNewLineUserControl = new WrappingAndNewLineUserControl();
                    wrappingAndNewLineUserControl.DataContext = UserSettings.MainInstance;
                }
                return wrappingAndNewLineUserControl;
            }
        }
    }
}
