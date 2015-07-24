using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting.Options
{
    public class IndentStyleInfo
    {
        /// <summary>
        /// Provides the indent styles for functions and tables
        /// </summary>
        /// <param name="generalIndentStyle">
        /// This is the general indent style for everything, if None is present here 
        /// </param>
        /// <param name="functionIndentStyle">
        /// This is the indent style for the function indent. This only affects functions that are written like:
        /// "foo = function()". This has no effect on functions like "function foo()"
        /// </param>
        /// <param name="tableIndentStyle">
        /// This is the indent style for table constructors.
        /// </param>
        public IndentStyleInfo(IndentStyle genernalIndentStyle, IndentStyle functionIndentStyle, IndentStyle tableIndentStyle)
        {
            this.Function = functionIndentStyle;
            this.Table = tableIndentStyle;
        }

        internal IndentStyle Function { get; }
        internal IndentStyle Table { get; }
    }
}
