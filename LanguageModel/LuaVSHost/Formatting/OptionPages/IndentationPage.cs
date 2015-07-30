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
    [Guid(Guids.IndentationPageString)]
    internal class IndentationPage : BaseDialogPage
    {
        private IndentationUserControl indentationUserControl;
        protected override UIElement Child
        {
            get
            {
                if (indentationUserControl == null)
                {
                    indentationUserControl = new IndentationUserControl();
                    indentationUserControl.DataContext = UserSettings.MainInstance;
                }
                return indentationUserControl;
            }
        }
    }
}
