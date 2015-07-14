using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting
{
    internal class FormattingContext
    {
        internal ParsedToken CurrentToken { get; private set; }
        internal ParsedToken NextToken { get; private set; }

        internal FormattingContext(ParsedToken currentToken, ParsedToken nextToken)
        {
            this.CurrentToken = currentToken;
            this.NextToken = nextToken;
        }

        internal bool IsTriviaInBetween(Trivia.TriviaType triviaType)
        {
            foreach (Trivia trivia in NextToken.Token.LeadingTrivia)
            {
                if (trivia.Type == triviaType)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool CommentsInBetween()
        {
            return this.IsTriviaInBetween(Trivia.TriviaType.Comment);
        }

        internal bool TokensOnSameLine()
        {
            return !this.IsTriviaInBetween(Trivia.TriviaType.Newline);
        }
        

    }
}
