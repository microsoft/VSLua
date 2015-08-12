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

        /// <summary>
        /// Formats the statenent made by the user. A statement as defined in the Formatting Functional Spec.
        /// </summary>
        // void FormatStatement();

        /// <summary>
        /// Formats the pasted content.
        /// </summary>
        void FormatOnPaste();
    }
}
