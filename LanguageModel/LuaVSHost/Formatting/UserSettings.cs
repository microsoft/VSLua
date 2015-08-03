using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting
{
    internal sealed class UserSettings : INotifyPropertyChanged
    {
        public static readonly UserSettings MainInstance = new UserSettings();

        public event PropertyChangedEventHandler PropertyChanged;

        private bool? formatOnEnter = true;
        public bool? FormatOnEnter
        {
            get
            {
                return this.formatOnEnter;
            }
            set
            {
                if (this.formatOnEnter != value)
                {
                    this.formatOnEnter = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? formatOnPaste = true;
        public bool? FormatOnPaste
        {
            get
            {
                return this.formatOnPaste;
            }
            set
            {
                if (this.formatOnPaste != value)
                {
                    this.formatOnPaste = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? formatOnBlock = true;
        public bool? FormatOnBlock
        {
            get
            {
                return this.formatOnBlock;
            }
            set
            {
                if (this.formatOnBlock != value)
                {
                    this.formatOnBlock = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private void RaisePropertyChangedEvent([CallerMemberName] string callingMember = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callingMember));
        }
    }
}
