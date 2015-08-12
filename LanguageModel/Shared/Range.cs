/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace LanguageService.Shared
{
    /// <summary>
    /// Species a number range
    /// </summary>
    public struct Range
    {
        /// <summary>
        /// Specifies a number range
        /// </summary>
        /// <param name="startFrom">The start index</param>
        /// <param name="endAt">The end index, must be equal or
        /// greater than <paramref name="startFrom"/></param>
        public Range(int startFrom, int endAt)
        {
            this.Start = startFrom;
            this.End = endAt;
        }

        internal int Start { get; }

        internal int End { get; }
    }
}
