using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Formatting.Options
{
    /// <summary>
    /// None - No indent formatting is applied
    /// Fixed - Indent is based soley on tab size given from the editor
    /// Relative - Relative only affects tables and functions, this is tokens are indented
    ///     based on the "function" key word and on the "{" on a table constructor. It does nothing
    ///     for any other indenting block.
    /// </summary>
    public enum IndentStyle
    {
        None,
        Fixed,
        Relative
    }
}
