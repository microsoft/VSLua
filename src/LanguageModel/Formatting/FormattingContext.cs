/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/
using Validation;

namespace LanguageService.Formatting
{
    internal struct FormattingContext
    {
        internal FormattingContext(ParsedToken currentToken, ParsedToken nextToken, SourceText sourceText)
        {
            Requires.NotNull(currentToken, nameof(currentToken));
            Requires.NotNull(nextToken, nameof(nextToken));
            Requires.NotNull(sourceText, nameof(sourceText));

            this.CurrentToken = currentToken;
            this.NextToken = nextToken;
            this.SourceText = sourceText;
        }

        internal ParsedToken CurrentToken { get; }

        internal ParsedToken NextToken { get; }

        internal SourceText SourceText { get; }

        internal bool TriviaBetweenTokensContains(SyntaxKind triviaType)
        {
            foreach (Trivia trivia in this.NextToken.Token.LeadingTrivia)
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
            return this.TriviaBetweenTokensContains(SyntaxKind.Comment);
        }

        internal bool TokensOnSameLine()
        {
            return !this.TriviaBetweenTokensContains(SyntaxKind.Newline);
        }
    }
}
