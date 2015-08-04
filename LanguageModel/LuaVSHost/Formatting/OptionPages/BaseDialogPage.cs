using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

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
