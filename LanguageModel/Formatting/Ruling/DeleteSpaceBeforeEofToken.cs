using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class DeleteSpaceBeforeEofToken : SimpleRule
    {
        internal DeleteSpaceBeforeEofToken() :
            base(
                new RuleDescriptor(TokenRange.All, TokenType.EndOfFile),
                new List<Func<FormattingContext, bool>> { Rules.TokensAreOnSameLine },
                RuleAction.Delete)
        {
        }

        internal override IEnumerable<TextEditInfo> Apply(FormattingContext formattingContext)
        {
            List<TextEditInfo> edits = new List<TextEditInfo>();
            TextEditInfo edit = this.GetLastSpaceTriviaInfo(formattingContext.NextToken.Token);

            if (edit != null)
            {
                edits.Add(edit);
            }
            return edits;
        }

        private TextEditInfo GetLastSpaceTriviaInfo(Token token)
        {
            List<Trivia> leadingTrivia = token.LeadingTrivia;
            if (leadingTrivia.Count == 0 || leadingTrivia == null)
            {
                return null;
            }

            Trivia lastTrivia = leadingTrivia[token.LeadingTrivia.Count - 1];

            TextEditInfo edit = null;

            if (lastTrivia.Type == Trivia.TriviaType.Whitespace)
            {
                int length = lastTrivia.Text.Length;
                int start = token.Start - length;
                edit = new TextEditInfo(start, length, "");
            }
            return edit;
        }

    }
}
