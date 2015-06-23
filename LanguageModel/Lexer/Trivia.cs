using System;

namespace LanguageModel
{
    public class Trivia
    {
        public enum TriviaType { Whitespace, Comment, Newline} // TODO: skippedtoken

        public TriviaType type { get; private set; }
        public string trivia { get; private set; }

        public Trivia(TriviaType type, string trivia)
        {
            this.type = type;
            this.trivia = trivia;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        private string MyToString()
        {
            return string.Format("{0}, {1}, {2}", Enum.GetName(typeof(TriviaType), type), trivia);
        }

    }
}
