using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.LuaLanguageService.Formatting.OptionPages.UserControls;
using Microsoft.VisualStudio.LuaLanguageService.Shared;

namespace Microsoft.VisualStudio.LuaLanguageService.Formatting.OptionPages
{
    [Guid(Guids.GeneralPageString)]
    internal class GeneralPage : BaseDialogPage
    {
        private GeneralUserControl generalUserControl;
        public GeneralPage()
        {
            generalUserControl = new GeneralUserControl();

            this.Bind(generalUserControl.formatOnEnterCheckbox, nameof(generalUserControl.formatOnEnterCheckbox.IsChecked),
                this.Core.UserSettings, nameof(this.Core.UserSettings.FormatOnEnter));

            this.Bind(generalUserControl.formatOnPasteCheckbox, nameof(generalUserControl.formatOnPasteCheckbox.IsChecked),
                this.Core.UserSettings, nameof(this.Core.UserSettings.FormatOnPaste));

            this.Bind(generalUserControl.formatOnBlockCheckbox, nameof(generalUserControl.formatOnBlockCheckbox.IsChecked),
                this.Core.UserSettings, nameof(this.Core.UserSettings.FormatOnBlock));
        }

        protected override UIElement Child
        {
            get
            {
                return generalUserControl;
            }
        }
    }
}
