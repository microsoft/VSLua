/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace LanguageService.Formatting.Ruling
{
    /// <summary>
    /// The set of enumerables that describes all the possible Rule actions
    /// </summary>
    internal enum RuleAction
    {
        /// <summary>
        /// Action that enforces one space between tokens
        /// </summary>
        Space,

        /// <summary>
        /// Action that removes spaces between tokens
        /// </summary>
        Delete,

        /// <summary>
        /// Action that inserts a newline between tokens
        /// </summary>
        Newline,

        /// <summary>
        /// Action that enforces nothing is done
        /// </summary>
        Ignore // <- Might not be needed
    }
}
