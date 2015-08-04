namespace LanguageService.Formatting.Options
{
    public sealed class IndentStyleInfo
    {
        /// <summary>
        /// Provides the indent styles for functions and tables
        /// </summary>
        /// <param name="functionIndentStyle">
        /// This is the indent style for the function indent. This only affects functions that are written like:
        /// "foo = function()". This has no effect on functions like "function foo()"
        /// </param>
        /// <param name="tableIndentStyle">
        /// This is the indent style for table constructors.
        /// </param>
        public IndentStyleInfo(IndentStyle functionIndentStyle, IndentStyle tableIndentStyle)
        {
            this.Function = functionIndentStyle;
            this.Table = tableIndentStyle;
        }
        internal IndentStyle Function { get; }
        internal IndentStyle Table { get; }
    }
}
