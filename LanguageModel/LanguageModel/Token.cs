using System.Collections.Generic;
using System.Text;

namespace LanguageService
{
    public class Token : SyntaxNodeOrToken
    {
        public int FullStart { get; private set; }
        public int Start { get; private set; }
        public string Text { get; private set; }
        public int Length { get; private set; }
        public int End
        {  get
            {
                return Start + Length - 1;
            }
        }
        public SyntaxKind Kind { get; private set; }

        public List<Trivia> LeadingTrivia { get; private set; } //TODO: change to Immutable List


        public Token(SyntaxKind tokentype, string value, List<Trivia> trivia, int fullStart, int start)
        {
            this.Kind = tokentype;
            this.Text = value;
            this.LeadingTrivia = trivia;
            this.FullStart = fullStart;
            this.Start = start;
            this.Length = Text.Length;
        }

        public static Token CreateMissingToken(int position)
        {
            return new Token(SyntaxKind.MissingToken, "", null, position, position);
        }

        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(this.Kind.ToString());
            sb.Append("\t");
            sb.Append(Text);

            return sb.ToString();
        }
    }
}
