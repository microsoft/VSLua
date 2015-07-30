using Microsoft.VisualStudio.LuaLanguageService.Shared;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.LuaLanguageService.Formatting.OptionPages
{
    internal abstract class BaseDialogPage : UIElementDialogPage
    {

        private ICore core;
        internal ICore Core
        {
            get
            {
                if (this.core == null)
                {
                    LuaLanguageService service = this.GetService(typeof(LuaLanguageService)) as LuaLanguageService;

                    if (service != null)
                    {
                        core = service.Core;
                    }
                }
                return this.core;
            }
        }

        public override object AutomationObject
        {
            get
            {
                return this.Core.UserSettings;
            }
        }

    }
}
