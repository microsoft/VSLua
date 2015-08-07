using Validation;

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
            Requires.NotNull(replaceWith, nameof(replaceWith));
            Requires.Argument(length >= 0, nameof(length), nameof(length) + " must be non-negative");
            Requires.Argument(start >= 0, nameof(start), nameof(start) + " must be non-negative");

            this.Start = start;
            this.Length = length;
            this.ReplacingWith = replaceWith;
        }
    }
}
