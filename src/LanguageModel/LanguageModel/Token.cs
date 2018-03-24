using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageService
{
    public class Token : SyntaxNodeOrToken
    {
        public int FullStart { get; private set; }
        public int Start { get; private set; }
        public string Text { get; private set; }
        public int Length { get; private set; }
        public int End => this.Start + this.Length;
        public SyntaxKind Kind { get; private set; }

        public List<Trivia> LeadingTrivia { get; private set; } //TODO: change to Immutable List

        public Token(SyntaxKind kind, string text, List<Trivia> trivia, int fullStart, int start)
        {
            this.Kind = kind;
            this.Text = text;
            this.Kind = kind;
            this.Text = text;
            this.LeadingTrivia = trivia == null ? new List<Trivia>() : trivia;
            this.FullStart = fullStart;
            this.Start = start;
            this.Length = this.Text.Length;
        }

        public static Token CreateMissingToken(int position)
        {
            return new Token(SyntaxKind.MissingToken, "", Enumerable.Empty<Trivia>().ToList(), position, position);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Kind.ToString());
            sb.Append("\t");
            sb.Append(this.Text);

            return sb.ToString();
        }

        public override System.Collections.Immutable.ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return null;
            }
        }
    }
}
