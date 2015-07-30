using System.Collections.Generic;
using System.Text;

namespace LanguageService
{
    public class Token /*TODO : SyntaxNodeOrToken*/
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
        public SyntaxKind Type { get; private set; }

        public List<Trivia> LeadingTrivia { get; private set; } //TODO: change to Immutable List


        public Token(SyntaxKind tokentype, string value, List<Trivia> trivia, int fullStart, int start)
        {
            this.Type = tokentype;
            this.Text = value;
            this.LeadingTrivia = trivia;
            this.FullStart = fullStart;
            this.Start = start;
            this.Length = Text.Length; //TODO: correct?
        }

        public static Token CreateMissingToken(int position)
        {
            return new Token(SyntaxKind.MissingToken, "", null, position, position);
        }

        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(this.Type.ToString());
            sb.Append("\t");
            sb.Append(Text);

            return sb.ToString();
        }
    }
}
