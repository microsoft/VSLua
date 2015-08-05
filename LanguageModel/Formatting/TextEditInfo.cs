using System;

namespace LanguageService.Formatting
{
    public class TextEditInfo
    {
        public int Start { get; }
        public int Length { get; }
        public string ReplacingString { get; }

        internal TextEditInfo(int start, int length, string replacingString)
        {
            if (replacingString == null)
            {
                throw new ArgumentNullException();
            }
            this.Start = start;
            this.Length = length;
            this.ReplacingString = replacingString;
        }
    }
}
