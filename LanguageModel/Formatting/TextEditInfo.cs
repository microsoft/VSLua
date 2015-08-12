using Validation;

namespace LanguageService.Formatting
{
    public class TextEditInfo
    {
        internal TextEditInfo(int start, int length, string replaceWith)
        {
            Requires.NotNull(replaceWith, nameof(replaceWith));
            Requires.Argument(length >= 0, nameof(length), nameof(length) + " must be non-negative");
            Requires.Argument(start >= 0, nameof(start), nameof(start) + " must be non-negative");

            this.Start = start;
            this.Length = length;
            this.ReplacingWith = replaceWith;
        }

        /// <summary>
        /// Gets the Start index of the text that is to be replaced.
        /// </summary>
        /// <value>The start of the text for the replacing string</value>
        public int Start { get; }

        /// <summary>
        /// Gets the length of the text from the start that is to be replaced.
        /// Length can be 0.
        /// </summary>
        /// <value>The length of the text that is to be replaced</value>
        public int Length { get; }

        /// <summary>
        /// Gets the string that will replace the text in question.
        /// </summary>
        /// <value>The string that spaces the text in question</value>
        public string ReplacingWith { get; }
    }
}
