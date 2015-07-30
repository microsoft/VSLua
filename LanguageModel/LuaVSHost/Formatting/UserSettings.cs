using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.LuaLanguageService.Formatting
{
    internal sealed class UserSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool? formatOnEnter = true;
        internal bool? FormatOnEnter
        {
            get
            {
                return formatOnEnter;
            }
            set
            {
                if (formatOnEnter != value)
                {
                    formatOnEnter = value;
                    RaisePropertyChangedEvent(FormatOnEnter, nameof(FormatOnEnter));
                }
            }
        }

        private bool? formatOnPaste = true;
        internal bool? FormatOnPaste
        {
            get
            {
                return formatOnPaste;
            }
            set
            {
                if (formatOnPaste != value)
                {
                    formatOnPaste = value;
                    RaisePropertyChangedEvent(FormatOnPaste, nameof(FormatOnPaste));
                }
            }
        }

        private bool? formatOnBlock = true;
        internal bool? FormatOnBlock
        {
            get
            {
                return formatOnBlock;
            }
            set
            {
                if (formatOnBlock != value)
                {
                    formatOnBlock = value;
                    RaisePropertyChangedEvent(FormatOnBlock, nameof(FormatOnBlock));
                }
            }
        }

        private void RaisePropertyChangedEvent(object sender, string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(name));
            }
        }
    }
}
