using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Options
{
    public enum IndentStyle
    {
        /// <summary>
        /// Fixed - Indent is based soley on tab size given from the editor
        /// </summary>
        Fixed,

        /// <summary>
        /// Relative - Relative only affects tables and functions, this is tokens are indented
        /// based on the "function" key word and on the "{" on a table constructor. It does nothing
        /// for any other indenting block.
        /// </summary>
        Relative
    }
}
