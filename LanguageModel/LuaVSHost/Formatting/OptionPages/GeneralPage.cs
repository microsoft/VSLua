using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LuaLanguageService.Shared;

namespace Microsoft.VisualStudio.LuaLanguageService.Formatting.OptionPages
{
    internal class GeneralPage : BaseDialogPage
    {

        private int optionInt = 256;

        [Category(Constants.Formatting.Category)]
        [DisplayName("My Integer Option")]
        [Description("My integer option")]
        public int OptionInteger
        {
            get { return optionInt; }
            set { optionInt = value; }
        }


        public override void SaveSettingsToStorage()
        {
            //base.SaveSettingsToStorage();
        }
    }
}
