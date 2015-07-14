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
                new RuleDescriptor(TokenRange.Any, TokenRange.Any),
                new List<ContextFilter> { Rules.TokensAreNotOnSameLine },
                RuleAction.Newline)
        {
        }

        internal override TextEditInfo Apply(FormattingContext formattingContext)
        {
            List<Trivia> leadingTrivia = formattingContext.NextToken.Token.LeadingTrivia;
            string replacingString = this.GetReplacingString(leadingTrivia);
            int start = formattingContext.CurrentToken.Token.Start + 
                formattingContext.CurrentToken.Token.Length;
            int length = formattingContext.NextToken.Token.Start - start;

            return new TextEditInfo(start, length, replacingString);
        }


        private string GetReplacingString(List<Trivia> leadingTrivia)
        {
            List<Trivia> newLeadingTrivia = new List<Trivia>();

            for (int i = 0; i < leadingTrivia.Count; ++i)
            {
                // this is to skip all whitespace that is before a newlines
                if (i + 1 < leadingTrivia.Count &&
                    leadingTrivia[i].Type == Trivia.TriviaType.Whitespace &&
                    leadingTrivia[i+1].Type == Trivia.TriviaType.Newline)
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
