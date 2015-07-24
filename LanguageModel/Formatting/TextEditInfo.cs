using System;

namespace LanguageService.Formatting
{
    public class TextEditInfo
    {
        /// <summary>
        /// The Start index of the text that is to be replaced.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// The length of the text from the start that is to be replaced.
        /// Length can be 0.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// The string that will replace the text in question.
        /// </summary>
        public string ReplacingWith { get; }

        internal TextEditInfo(int start, int length, string replaceWith)
        {
            Validation.Requires.NotNull(replaceWith, nameof(replaceWith));
            Validation.Assumes.True(length >= 0, nameof(length) + " must be non-negative");
            Validation.Assumes.True(start >= 0, nameof(start) + " must be non-negative");

            this.Start = start;
            this.Length = length;
            this.ReplacingWith = replaceWith;
        }
    }
}
