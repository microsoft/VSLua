using Microsoft.VisualStudio.LanguageServices.Lua.Shared;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ComponentModelHost;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages
{
    internal abstract class BaseDialogPage : UIElementDialogPage
    {
        public override object AutomationObject
        {
            get
            {
                return UserSettings.MainInstance;
            }
        }

    }
}
