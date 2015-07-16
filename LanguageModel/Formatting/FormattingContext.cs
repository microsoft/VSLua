using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageService.Formatting
{
    internal struct FormattingContext
    {
        internal ParsedToken CurrentToken { get; }
        internal ParsedToken NextToken { get; }

        internal FormattingContext(ParsedToken currentToken, ParsedToken nextToken)
        {
            if (currentToken == null || nextToken == null)
            {
                throw new ArgumentNullException();
            }
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

        internal bool ContainsCommentsBetweenTokens()
        {
            return this.IsTriviaInBetween(Trivia.TriviaType.Comment);
        }

        internal bool TokensOnSameLine()
        {
            return !this.IsTriviaInBetween(Trivia.TriviaType.Newline);
        }
    }
}
