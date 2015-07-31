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

        private bool? spaceAfterCommas = true;
        public bool? SpaceAfterCommas
        {
            get
            {
                return spaceAfterCommas;
            }
            set
            {
                if (spaceAfterCommas != value)
                {
                    spaceAfterCommas = value;
                    RaisePropertyChangedEvent(SpaceAfterCommas, nameof(SpaceAfterCommas));
                }
            }
        }

        private bool? spaceBeforeAndAfterBinaryOperations = true;
        public bool? SpaceBeforeAndAfterBinaryOperations
        {
            get
            {
                return spaceBeforeAndAfterBinaryOperations;
            }
            set
            {
                if (spaceBeforeAndAfterBinaryOperations != value)
                {
                    spaceBeforeAndAfterBinaryOperations = value;
                    RaisePropertyChangedEvent(SpaceBeforeAndAfterBinaryOperations, nameof(SpaceBeforeAndAfterBinaryOperations));
                }
            }
        }

        private bool? spaceBeforeAndAfterAssignmentOperatorOnField = true;
        public bool? SpaceBeforeAndAfterAssignmentOperatorOnField
        {
            get
            {
                return spaceBeforeAndAfterAssignmentOperatorOnField;
            }
            set
            {
                if (spaceBeforeAndAfterAssignmentOperatorOnField != value)
                {
                    spaceBeforeAndAfterAssignmentOperatorOnField = value;
                    RaisePropertyChangedEvent(SpaceBeforeAndAfterAssignmentOperatorOnField, nameof(SpaceBeforeAndAfterAssignmentOperatorOnField));
                }
            }
        }

        private bool? spaceBeforeAndAfterAssignmentInStatement = true;
        public bool? SpaceBeforeAndAfterAssignmentInStatement
        {
            get
            {
                return spaceBeforeAndAfterAssignmentInStatement;
            }
            set
            {
                if (spaceBeforeAndAfterAssignmentInStatement != value)
                {
                    spaceBeforeAndAfterAssignmentInStatement = value;
                    RaisePropertyChangedEvent(SpaceBeforeAndAfterAssignmentInStatement, nameof(SpaceBeforeAndAfterAssignmentInStatement));
                }
            }
        }

        private bool? forLoopAssignmentSpacing = false;
        public bool? ForLoopAssignmentSpacing
        {
            get
            {
                return forLoopAssignmentSpacing;
            }
            set
            {
                if (forLoopAssignmentSpacing != value)
                {
                    forLoopAssignmentSpacing = value;
                    RaisePropertyChangedEvent(ForLoopAssignmentSpacing, nameof(ForLoopAssignmentSpacing));
                }
            }
        }

        private bool? forLoopIndexSpacing = false;
        public bool? ForLoopIndexSpacing
        {
            get
            {
                return forLoopIndexSpacing;
            }
            set
            {
                if (forLoopIndexSpacing != value)
                {
                    forLoopIndexSpacing = value;
                    RaisePropertyChangedEvent(ForLoopIndexSpacing, nameof(ForLoopIndexSpacing));
                }
            }
        }

        private bool? spaceBetweenFunctionAndParenthesis = true;
        public bool? SpaceBetweenFunctionAndParenthesis
        {
            get
            {
                return spaceBetweenFunctionAndParenthesis;
            }
            set
            {
                if (spaceBetweenFunctionAndParenthesis != value)
                {
                    spaceBetweenFunctionAndParenthesis = value;
                    RaisePropertyChangedEvent(SpaceBetweenFunctionAndParenthesis, nameof(SpaceBetweenFunctionAndParenthesis));
                }
            }
        }

        private bool? addSpacesOnInsideOfParenthesis = false;
        public bool? AddSpacesOnInsideOfParenthesis
        {
            get
            {
                return addSpacesOnInsideOfParenthesis;
            }
            set
            {
                if (addSpacesOnInsideOfParenthesis != value)
                {
                    addSpacesOnInsideOfParenthesis = value;
                    RaisePropertyChangedEvent(AddSpacesOnInsideOfParenthesis, nameof(AddSpacesOnInsideOfParenthesis));
                }
            }
        }

        private bool? addSpacesOnInsideOfCurlyBraces = true;
        public bool? AddSpacesOnInsideOfCurlyBraces
        {
            get
            {
                return addSpacesOnInsideOfCurlyBraces;
            }
            set
            {
                if (addSpacesOnInsideOfCurlyBraces != value)
                {
                    addSpacesOnInsideOfCurlyBraces = value;
                    RaisePropertyChangedEvent(AddSpacesOnInsideOfCurlyBraces, nameof(AddSpacesOnInsideOfCurlyBraces));
                }
            }
        }

        private bool? addSpacesOnInsideOfSquareBrackets = false;
        public bool? AddSpacesOnInsideOfSquareBrackets
        {
            get
            {
                return addSpacesOnInsideOfSquareBrackets;
            }
            set
            {
                if (addSpacesOnInsideOfSquareBrackets != value)
                {
                    addSpacesOnInsideOfSquareBrackets = value;
                    RaisePropertyChangedEvent(AddSpacesOnInsideOfSquareBrackets, nameof(AddSpacesOnInsideOfSquareBrackets));
                }
            }
        }

        private bool? wrapSingleLineFunctions = false;
        public bool? WrapSingleLineFunctions
        {
            get
            {
                return wrapSingleLineFunctions;
            }
            set
            {
                if (wrapSingleLineFunctions != value)
                {
                    wrapSingleLineFunctions = value;
                    RaisePropertyChangedEvent(WrapSingleLineFunctions, nameof(WrapSingleLineFunctions));
                }
            }
        }

        private bool? wrapSingleLineForLoops = false;
        public bool? WrapSingleLineForLoops
        {
            get
            {
                return wrapSingleLineForLoops;
            }
            set
            {
                if (wrapSingleLineForLoops != value)
                {
                    wrapSingleLineForLoops = value;
                    RaisePropertyChangedEvent(WrapSingleLineForLoops, nameof(WrapSingleLineForLoops));
                }
            }
        }

        private bool? wrapSingleLineTableConstructors = false;
        public bool? WrapSingleLineTableConstructors
        {
            get
            {
                return wrapSingleLineTableConstructors;
            }
            set
            {
                if (wrapSingleLineTableConstructors != value)
                {
                    wrapSingleLineTableConstructors = value;
                    RaisePropertyChangedEvent(WrapSingleLineTableConstructors, nameof(WrapSingleLineTableConstructors));
                }
            }
        }

        private bool? addNewLinesToMultilineTableConstructors = false;
        public bool? AddNewLinesToMultilineTableConstructors
        {
            get
            {
                return addNewLinesToMultilineTableConstructors;
            }
            set
            {
                if (addNewLinesToMultilineTableConstructors != value)
                {
                    addNewLinesToMultilineTableConstructors = value;
                    RaisePropertyChangedEvent(AddNewLinesToMultilineTableConstructors, nameof(AddNewLinesToMultilineTableConstructors));
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
