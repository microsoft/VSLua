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
        /// <param name="start">The start index</param>
        /// <param name="length">The end index, must be equal or
        /// greater than <paramref name="start"/></param>
        public Range(int start, int length)
        {
            this.Start = start;
            this.Length = length;
            this.End = start + length;
        }

        public int Start { get; }

        public int End { get; }

        public int Length { get; }
    }
}
