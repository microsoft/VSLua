using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting
{
    internal sealed class UserSettings : INotifyPropertyChanged
    {
        public static readonly UserSettings MainInstance = new UserSettings();

        internal UserSettings()
        {
            this.RulesChanged = false;
        }

        internal bool RulesChanged { get; private set; }

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

        private bool? functionFixedIndentation = true;
        public bool? FunctionFixedIndentation
        {
            get
            {
                return this.functionFixedIndentation;
            }
            set
            {
                if (this.functionFixedIndentation != value)
                {
                    this.functionFixedIndentation = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? functionRelativeIndentation = false;
        public bool? FunctionRelativeIndentation
        {
            get
            {
                return this.functionRelativeIndentation;
            }
            set
            {
                if (this.functionRelativeIndentation != value)
                {
                    this.functionRelativeIndentation = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? tableFixedIndentation = true;
        public bool? TableFixedIndentation
        {
            get
            {
                return this.tableFixedIndentation;
            }
            set
            {
                if (this.tableFixedIndentation != value)
                {
                    this.tableFixedIndentation = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? tableRelativeIndentation = false;
        public bool? TableRelativeIndentation
        {
            get
            {
                return this.tableRelativeIndentation;
            }
            set
            {
                if (this.tableRelativeIndentation != value)
                {
                    this.tableRelativeIndentation = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? spaceAfterCommas = true;
        public bool? SpaceAfterCommas
        {
            get
            {
                return this.spaceAfterCommas;
            }
            set
            {
                if (this.spaceAfterCommas != value)
                {
                    this.spaceAfterCommas = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? spaceBeforeAndAfterBinaryOperations = true;
        public bool? SpaceBeforeAndAfterBinaryOperations
        {
            get
            {
                return this.spaceBeforeAndAfterBinaryOperations;
            }
            set
            {
                if (this.spaceBeforeAndAfterBinaryOperations != value)
                {
                    this.spaceBeforeAndAfterBinaryOperations = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? spaceBeforeAndAfterAssignmentOperatorOnField = true;
        public bool? SpaceBeforeAndAfterAssignmentOperatorOnField
        {
            get
            {
                return this.spaceBeforeAndAfterAssignmentOperatorOnField;
            }
            set
            {
                if (this.spaceBeforeAndAfterAssignmentOperatorOnField != value)
                {
                    this.spaceBeforeAndAfterAssignmentOperatorOnField = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? spaceBeforeAndAfterAssignmentInStatement = true;
        public bool? SpaceBeforeAndAfterAssignmentInStatement
        {
            get
            {
                return this.spaceBeforeAndAfterAssignmentInStatement;
            }
            set
            {
                if (this.spaceBeforeAndAfterAssignmentInStatement != value)
                {
                    this.spaceBeforeAndAfterAssignmentInStatement = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? forLoopAssignmentSpacing = false;
        public bool? ForLoopAssignmentSpacing
        {
            get
            {
                return this.forLoopAssignmentSpacing;
            }
            set
            {
                if (this.forLoopAssignmentSpacing != value)
                {
                    this.forLoopAssignmentSpacing = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? forLoopIndexSpacing = false;
        public bool? ForLoopIndexSpacing
        {
            get
            {
                return this.forLoopIndexSpacing;
            }
            set
            {
                if (this.forLoopIndexSpacing != value)
                {
                    this.forLoopIndexSpacing = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? spaceBetweenFunctionAndParenthesis = true;
        public bool? SpaceBetweenFunctionAndParenthesis
        {
            get
            {
                return this.spaceBetweenFunctionAndParenthesis;
            }
            set
            {
                if (this.spaceBetweenFunctionAndParenthesis != value)
                {
                    this.spaceBetweenFunctionAndParenthesis = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? addSpacesOnInsideOfParenthesis = false;
        public bool? AddSpacesOnInsideOfParenthesis
        {
            get
            {
                return this.addSpacesOnInsideOfParenthesis;
            }
            set
            {
                if (this.addSpacesOnInsideOfParenthesis != value)
                {
                    this.addSpacesOnInsideOfParenthesis = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? addSpacesOnInsideOfCurlyBraces = true;
        public bool? AddSpacesOnInsideOfCurlyBraces
        {
            get
            {
                return this.addSpacesOnInsideOfCurlyBraces;
            }
            set
            {
                if (this.addSpacesOnInsideOfCurlyBraces != value)
                {
                    this.addSpacesOnInsideOfCurlyBraces = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? addSpacesOnInsideOfSquareBrackets = false;
        public bool? AddSpacesOnInsideOfSquareBrackets
        {
            get
            {
                return this.addSpacesOnInsideOfSquareBrackets;
            }
            set
            {
                if (this.addSpacesOnInsideOfSquareBrackets != value)
                {
                    this.addSpacesOnInsideOfSquareBrackets = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? wrapSingleLineFunctions = false;
        public bool? WrapSingleLineFunctions
        {
            get
            {
                return this.wrapSingleLineFunctions;
            }
            set
            {
                if (this.wrapSingleLineFunctions != value)
                {
                    this.wrapSingleLineFunctions = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? wrapSingleLineForLoops = false;
        public bool? WrapSingleLineForLoops
        {
            get
            {
                return this.wrapSingleLineForLoops;
            }
            set
            {
                if (this.wrapSingleLineForLoops != value)
                {
                    this.wrapSingleLineForLoops = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? wrapSingleLineTableConstructors = false;
        public bool? WrapSingleLineTableConstructors
        {
            get
            {
                return this.wrapSingleLineTableConstructors;
            }
            set
            {
                if (this.wrapSingleLineTableConstructors != value)
                {
                    this.wrapSingleLineTableConstructors = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private bool? addNewLinesToMultilineTableConstructors = false;
        public bool? AddNewLinesToMultilineTableConstructors
        {
            get
            {
                return this.addNewLinesToMultilineTableConstructors;
            }
            set
            {
                if (this.addNewLinesToMultilineTableConstructors != value)
                {
                    this.addNewLinesToMultilineTableConstructors = value;
                    this.RaisePropertyChangedEvent();
                }
            }
        }

        private void RaisePropertyChangedEvent([CallerMemberName] string callingMember = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callingMember));
            this.RulesChanged = true;
        }
    }
}