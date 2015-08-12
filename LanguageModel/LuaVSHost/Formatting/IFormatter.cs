/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace Microsoft.VisualStudio.LanguageServices.Lua.Formatting
{
    internal interface IFormatter
    {
        /// <summary>
        /// Formats the entire document.
        /// </summary>
        void FormatDocument();

        /// <summary>
        /// Formats the line based on the caret position.
        /// </summary>
        void FormatOnEnter();

        /// <summary>
        /// Formats the selection made by the user.
        /// </summary>
        void FormatSelection();

#pragma warning disable SA1512, SA1515 // Single-line comments must not be followed/preceeded by blank line
        /// <summary>
        /// Formats the statenent made by the user. A statement as defined in the Formatting Functional Spec.
        /// </summary>

        // void FormatStatement();

        /// <summary>
        /// Formats the pasted content.
        /// </summary>
        void FormatOnPaste();
#pragma warning restore SA1512, SA1515 // Single-line comments must not be followed/preceeded by blank line
    }
}
