using System;

namespace LanguageService
{
    public class Trivia
    {
        public enum TriviaType { Whitespace, Comment, Newline} // TODO: skippedtoken

        public TriviaType Type { get; private set; }
        public string Text { get; private set; }

        public Trivia(TriviaType type, string trivia)
        {
            this.Type = type;
            this.Text = trivia;
        }

        public string ToString()
        {
            return string.Format("{0},\t{1}", Enum.GetName(typeof(TriviaType), Type), ConvertStringToSymbols(Text));
        }

        private string ConvertStringToSymbols(string s) //TODO: temp, just for testing
        {
            string new_string = "";

            foreach (char c in s)
            {
                if (c == '\r')
                {
                    new_string += "\\r";
                }
                else if (c == '\n')
                {
                    new_string += "\\n";
                }
                else if (c == '\t')
                {
                    new_string += "\\t";
                }
                else if (c == ' ')
                {
                    new_string += "\\s";
                }
                else
                {
                    new_string += c;
                }
            }

            return new_string;
        }

    }
}
