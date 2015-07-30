using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServices.Lua.Formatting.OptionPages;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting
{
    internal sealed class UserSettings : INotifyPropertyChanged
    {
        public readonly static UserSettings MainInstance = new UserSettings();

        public event PropertyChangedEventHandler PropertyChanged;

        private bool? formatOnEnter = true;
        public bool? FormatOnEnter
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
        public bool? FormatOnPaste
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
        public bool? FormatOnBlock
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

        private bool? functionFixedIndentation = true;
        public bool? FunctionFixedIndentation
        {
            get
            {
                return functionFixedIndentation;
            }
            set
            {
                if (functionFixedIndentation != value)
                {
                    functionFixedIndentation = value;
                    RaisePropertyChangedEvent(FunctionFixedIndentation, nameof(FunctionFixedIndentation));
                }
            }
        }

        private bool? functionRelativeIndentation = false;
        public bool? FunctionRelativeIndentation
        {
            get
            {
                return functionRelativeIndentation;
            }
            set
            {
                if (functionRelativeIndentation != value)
                {
                    functionRelativeIndentation = value;
                    RaisePropertyChangedEvent(FunctionRelativeIndentation, nameof(FunctionRelativeIndentation));
                }
            }
        }

        private bool? tableFixedIndentation = true;
        public bool? TableFixedIndentation
        {
            get
            {
                return tableFixedIndentation;
            }
            set
            {
                if (tableFixedIndentation != value)
                {
                    tableFixedIndentation = value;
                    RaisePropertyChangedEvent(TableFixedIndentation, nameof(TableFixedIndentation));
                }
            }
        }

        private bool? tableRelativeIndentation = false;
        public bool? TableRelativeIndentation
        {
            get
            {
                return tableRelativeIndentation;
            }
            set
            {
                if (tableRelativeIndentation != value)
                {
                    tableRelativeIndentation = value;
                    RaisePropertyChangedEvent(TableRelativeIndentation, nameof(TableRelativeIndentation));
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
