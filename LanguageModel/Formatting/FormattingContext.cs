namespace LanguageService.Formatting
{
    internal struct FormattingContext
    {
        internal FormattingContext(ParsedToken currentToken, ParsedToken nextToken)
        {
            Validation.Requires.NotNull(currentToken, nameof(currentToken));
            Validation.Requires.NotNull(nextToken, nameof(nextToken));

            this.CurrentToken = currentToken;
            this.NextToken = nextToken;
        }

        internal ParsedToken CurrentToken { get; }
        internal ParsedToken NextToken { get; }

        internal bool TriviaBetweenTokensContains(Trivia.TriviaType triviaType)
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
            return this.TriviaBetweenTokensContains(Trivia.TriviaType.Comment);
        }

        internal bool TokensOnSameLine()
        {
            return !this.TriviaBetweenTokensContains(Trivia.TriviaType.Newline);
        }
    }
}
