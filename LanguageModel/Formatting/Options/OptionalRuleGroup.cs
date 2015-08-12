/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace LanguageService.Formatting.Options
{
    /// <summary>
    /// The OptionalRuleGroups represent sets of rules that can enable/disable formatting options.
    /// </summary>
    public enum OptionalRuleGroup
    {
        /// <summary>
        /// The set of rules that make up the wrapping in for loops (single-lined)
        /// </summary>
        WrappingOneLineForFors,

        /// <summary>
        /// The set of rules that make up the wrapping in functions (single-lined)
        /// </summary>
        WrappingOneLineForFunctions,

        /// <summary>
        /// The set of rules that make up the wrapping for table constructors (single-lined)
        /// </summary>
        WrappingOneLineForTableConstructors,

        /// <summary>
        /// The set of rules that make up the wrapping for table constructors (multi-lined)
        /// </summary>
        WrappingMoreLinesForTableConstructors,

        /// <summary>
        /// The set of individual rules that insert a space before open parenthesis'
        /// </summary>
        SpaceBeforeOpenParenthesis,

        /// <summary>
        /// The set of rules that insert spaces inside of parenthesis'
        /// </summary>
        SpaceOnInsideOfParenthesis,

        /// <summary>
        /// The set of rules that insert spaces inside of curly braces
        /// </summary>
        SpaceOnInsideOfCurlyBraces,

        /// <summary>
        /// The set of rules that insert spaces inside of square brackets
        /// </summary>
        SpaceOnInsideOfSquareBrackets,

        /// <summary>
        /// The set of rules that insert spaces after commas
        /// </summary>
        SpaceAfterCommas,

        /// <summary>
        /// The set of rules that insert spaces before and after binary operations
        /// </summary>
        SpaceBeforeAndAfterBinaryOperations,

        /// <summary>
        /// The set of rules that insert spaces before and after assignment operators in fields
        /// </summary>
        SpaceBeforeAndAfterAssignmentForField,

        /// <summary>
        /// The set of rules that insert spaces before and after asssignment operators in statements
        /// </summary>
        SpaceBeforeAndAfterAssignmentForStatement,

        /// <summary>
        /// The set of rules that insert spaces before and after assignment in for loops
        /// </summary>
        SpaceBeforeAndAfterAssignmentInForLoopHeader,

        /// <summary>
        /// The set of rules that delete spaces after indicies in the for loop header
        /// </summary>
        NoSpaceBeforeAndAfterIndiciesInForLoopHeader
    }
}
