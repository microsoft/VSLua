using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageModel
{
    internal class Token
    {
        public enum TokenType //TODO: consider multi-line formatting?
        {
            Keyword, Operator, Identifier, Punctuation, OpenBracket,
            CloseBracket, OpenParen, CloseParen, OpenCurlyBrace,
            CloseCurlyBrace, Number, String, Unknown, EndOfFile
        }

        protected int fullStart;
        protected int start;
        protected string value;
        public TokenType type { get; private set; }
        protected List<Trivia> leadingTrivia;


        public Token(TokenType tokentype, string value, List<Trivia> trivia, int fullStart, int start)
        {
            this.type = tokentype;
            this.value = value;
            this.leadingTrivia = trivia;
            this.fullStart = fullStart;
            this.start = start;
        }

        public string ToString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("==============================================\nTrivia:\n");

            foreach (Trivia triv in leadingTrivia)
            {
                sb.Append("\t");
                sb.Append(Enum.GetName(typeof(Trivia.TriviaType), triv.type));
                sb.Append("\t" + triv.trivia);
                sb.Append("\n");
            }

            sb.Append("Data:\n\t");
            sb.Append(this.type.ToString());
            sb.Append("\t");
            sb.Append(value);
            sb.Append("\t");
            sb.Append(start);

            return sb.ToString();
        }
    }
}
