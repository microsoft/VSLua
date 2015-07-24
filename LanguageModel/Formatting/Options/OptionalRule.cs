using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Options
{
    public enum OptionalRule
    {
        WrappingOneLineForFors,
        WrappingOneLineForFunctions,
        WrappingOneLineForTableConstructors,
        WrappingMoreLinesForTableConstructors,
        SpaceBeforeOpenParenthesis,
        SpaceOnInsideOfParenthesis,
        SpaceOnInsideOfCurlyBrackets,
        SpaceOnInsideOfSquareBrackets,
        SpaceAfterCommas,
        SpaceBeforeAndAfterBinaryOperations,
        SpaceBeforeAndAfterAssignmentForField,
        SpaceBeforeAndAfterAssignmentForStatment,
        FormattingInFor
    }
}
