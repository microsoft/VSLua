/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace LanguageService.Classification
{
    /// <summary>
    /// Defines the token classifications for the Lua language service, every token can be placed into
    /// one of these categories, all tokens only have one classification.
    /// </summary>
    public enum Classification
    {
        /// <summary>
        /// Specifies a Keyword type as defined in the Lua Language Service
        /// </summary>
        Keyword,

        /// <summary>
        /// Any name that is not a keyword
        /// </summary>
        Identifier,

        /// <summary>
        /// Dots, Semi-Colons, Colons, Commas
        /// </summary>
        Punctuation,

        /// <summary>
        /// A number as defined in the Lua reference manual
        /// </summary>
        Number,

        /// <summary>
        /// A string literal as defined in the Lua reference manual
        /// </summary>
        StringLiteral,

        /// <summary>
        /// Both multiline and single-lined comments
        /// </summary>
        Comment,

        /// <summary>
        /// Operators as defined in the Lua reference manual, excludes "and" and "or" as keywords.
        /// </summary>
        Operator,

        /// <summary>
        /// true, false, nil
        /// </summary>
        KeyValue,

        /// <summary>
        /// Square brackets, curly braces, parenthesis
        /// </summary>
        Bracket
    }
}
