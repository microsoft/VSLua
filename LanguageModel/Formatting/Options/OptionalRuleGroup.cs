using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Options
{
    public enum OptionalRuleGroup
    {
        WrappingOneLineForFors,
        WrappingOneLineForFunctions,
        WrappingOneLineForTableConstructors,
        WrappingMoreLinesForTableConstructors,
        SpaceBeforeOpenParenthesis,
        SpaceOnInsideOfParenthesis,
        SpaceOnInsideOfCurlyBraces,
        SpaceOnInsideOfSquareBrackets,
        SpaceAfterCommas,
        SpaceBeforeAndAfterBinaryOperations,
        SpaceBeforeAndAfterAssignmentForField,
        SpaceBeforeAndAfterAssignmentForStatement,
        FormattingInFor
    }
}
