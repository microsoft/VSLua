using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting.Ruling
{
    internal class RuleTrailingWhitespace : Rule
    {
        internal RuleTrailingWhitespace() :
            base(
                new RuleDescriptor(TokenRange.All, TokenRange.All),
                new List<ContextFilter> { Rules.TokensAreNotOnSameLine },
                RuleAction.Newline)
        {
        }

        internal override TextEditInfo Apply(FormattingContext formattingContext)
        {
            List<Trivia> leadingTrivia = formattingContext.NextToken.Token.LeadingTrivia;

            // flag for the end of file token where whitespace should be skipped
            TokenType nextTokenType = formattingContext.NextToken.Token.Type;
            string replacingString = this.GetReplacingString(leadingTrivia, nextTokenType == TokenType.EndOfFile);
            int start = formattingContext.CurrentToken.Token.Start +
                formattingContext.CurrentToken.Token.Length;
            int length = formattingContext.NextToken.Token.Start - start;

            return new TextEditInfo(start, length, replacingString);
        }


        private string GetReplacingString(List<Trivia> leadingTrivia, bool isEndOfFile)
        {
            List<Trivia> newLeadingTrivia = new List<Trivia>();

            for (int i = 0; i < leadingTrivia.Count; ++i)
            {
                // this is to skip all whitespace that is before a newline
                if ((i + 1 < leadingTrivia.Count) &&
                    leadingTrivia[i].Type == Trivia.TriviaType.Whitespace &&
                    leadingTrivia[i + 1].Type == Trivia.TriviaType.Newline)
                {
                    continue;
                }

                // this is to not add the trailing whitespace at the end of a file BEFORE Eof
                if (i + 1 == leadingTrivia.Count && isEndOfFile &&
                    leadingTrivia[i].Type == Trivia.TriviaType.Whitespace)
                {
                    continue;
                }

                newLeadingTrivia.Add(leadingTrivia[i]);
            }

            return this.BuildStringFromLeadingTrivia(newLeadingTrivia);
        }

        private string BuildStringFromLeadingTrivia(List<Trivia> leadingTrivia)
        {
            StringBuilder triviaString = new StringBuilder();
            foreach (Trivia trivia in leadingTrivia)
            {
                triviaString.Append(trivia.Text);
            }

            return triviaString.ToString();

        }
    }
}
