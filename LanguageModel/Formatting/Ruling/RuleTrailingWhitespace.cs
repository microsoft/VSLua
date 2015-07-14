using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class DeleteTrailingWhitespaceRule : SimpleRule
    {
        internal DeleteTrailingWhitespaceRule() :
            base(
                new RuleDescriptor(TokenRange.All, TokenRange.All),
                new List<ContextFilter> { Rules.TokensAreNotOnSameLine },
                RuleAction.Delete)
        {
        }

        internal override List<TextEditInfo> Apply(FormattingContext formattingContext)
        {
            Token nextToken = formattingContext.NextToken.Token;

            // flag for the end of file token where whitespace should be skipped
            List<TextEditInfo> edits = this.GetEdits(nextToken, nextToken.Type == TokenType.EndOfFile);

            return edits;
        }


        private List<TextEditInfo> GetEdits(Token token, bool isEndOfFile)
        {

            List<TextEditInfo> edits = new List<TextEditInfo>();

            int start = token.FullStart;
            int length = 0;
            var leadingTrivia = token.LeadingTrivia;
            for (int i = 0; i < leadingTrivia.Count; ++i)
            {
                length = leadingTrivia[i].Text.Length;
                
                if (
                    // this is to delete all whitespace that is before a newline
                    (i + 1 < leadingTrivia.Count &&
                    leadingTrivia[i].Type == Trivia.TriviaType.Whitespace &&
                    leadingTrivia[i + 1].Type == Trivia.TriviaType.Newline) ||
                    // this is to delete the trailing whitespace at the end of a file BEFORE Eof
                    (i + 1 == leadingTrivia.Count && isEndOfFile &&
                    leadingTrivia[i].Type == Trivia.TriviaType.Whitespace))
                {
                    edits.Add(new TextEditInfo(start, length, ""));
                }

                start += length;
            }

            return edits;
        }
    }
}
